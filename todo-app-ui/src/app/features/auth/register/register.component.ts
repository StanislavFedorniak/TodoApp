import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { email, form, FormField, FormRoot, maxLength, minLength, required } from '@angular/forms/signals';
import { firstValueFrom } from 'rxjs';
import { RegisterDto } from '../../../core/models/auth.model';
import { AuthService } from '../../../core/services/auth.service';
import { getHttpErrorMessage } from '../../../core/utils/http-error-message';

@Component({
  selector: 'app-register',
  imports: [FormField, FormRoot, RouterLink],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly error = signal<string | null>(null);

  private readonly model = signal<RegisterDto>({ email: '', password: '' });

  readonly registerForm = form(
    this.model,
    path => {
      required(path.email, { message: 'Email is required.' });
      email(path.email, { message: 'Enter a valid email address.' });
      maxLength(path.email, 256, { message: 'Email must be at most 256 characters.' });
      required(path.password, { message: 'Password is required.' });
      minLength(path.password, 8, { message: 'Password must be at least 8 characters.' });
      maxLength(path.password, 100, { message: 'Password must be at most 100 characters.' });
    },
    {
      submission: {
        action: async () => {
          try {
            await firstValueFrom(this.auth.register(this.model()));
          } catch (err: unknown) {
            this.error.set(getHttpErrorMessage(err, 'Failed to create the account.'));
            return;
          }
          await this.router.navigate(['/']);
        }
      }
    }
  );
}
