import { HttpErrorResponse } from '@angular/common/http';

interface ProblemDetails {
  detail?: string;
  errors?: Record<string, string[]>;
}

export function getHttpErrorMessage(error: unknown, fallback: string): string {
  if (!(error instanceof HttpErrorResponse)) {
    return fallback;
  }

  if (error.status === 0) {
    return 'Cannot reach the server. Check your connection and try again.';
  }

  if (error.status === 429) {
    const seconds = Number(error.headers.get('Retry-After'));
    return Number.isFinite(seconds) && seconds > 0
      ? `Too many attempts. Try again in ${seconds} seconds.`
      : 'Too many attempts. Try again later.';
  }

  const problem = error.error as ProblemDetails | null;
  const validationMessage = problem?.errors ? Object.values(problem.errors)[0]?.[0] : undefined;

  return validationMessage || problem?.detail || fallback;
}
