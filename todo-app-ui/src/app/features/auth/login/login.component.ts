import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { email, form, FormField, FormRoot, required } from '@angular/forms/signals';
import { firstValueFrom } from 'rxjs';
import { LoginDto } from '../../../core/models/auth.model';
import { AuthService } from '../../../core/services/auth.service';
import { getHttpErrorMessage } from '../../../core/utils/http-error-message';

@Component({
  selector: 'app-login',
  imports: [FormField, FormRoot, RouterLink],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  private readonly auth = inject(AuthService);
  private readonly router = inject(Router);

  readonly error = signal<string | null>(null);

  private readonly model = signal<LoginDto>({ email: '', password: '' });

  readonly loginForm = form(
    this.model,
    path => {
      required(path.email, { message: 'Email is required.' });
      email(path.email, { message: 'Enter a valid email address.' });
      required(path.password, { message: 'Password is required.' });
    },
    {
      submission: {
        action: async () => {
          try {
            await firstValueFrom(this.auth.login(this.model()));
          } catch (err: unknown) {
            this.error.set(getHttpErrorMessage(err, 'Failed to sign in.'));
            return;
          }
          await this.router.navigate(['/']);
        }
      }
    }
  );
}
