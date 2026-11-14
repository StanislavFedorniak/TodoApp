import { Component, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { form, FormField, FormRoot, required } from '@angular/forms/signals';
import { firstValueFrom } from 'rxjs';
import { AccountDeleteDto } from '../../core/models/account.model';
import { AccountService } from '../../core/services/account.service';
import { AuthService } from '../../core/services/auth.service';
import { getHttpErrorMessage } from '../../core/utils/http-error-message';

@Component({
  selector: 'app-account',
  imports: [FormField, FormRoot],
  templateUrl: './account.component.html'
})
export class AccountComponent {
  private readonly accountService = inject(AccountService);
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly error = signal<string | null>(null);

  private readonly model = signal<AccountDeleteDto>({ password: '' });

  readonly deleteForm = form(
    this.model,
    path => {
      required(path.password, { message: 'Enter your password to confirm.' });
    },
    {
      submission: {
        action: async () => {
          try {
            await firstValueFrom(this.accountService.delete(this.model()));
          } catch (err: unknown) {
            this.error.set(getHttpErrorMessage(err, 'Failed to delete the account.'));
            return;
          }
          this.auth.logout();
          await this.router.navigate(['/login']);
        }
      }
    }
  );
}
