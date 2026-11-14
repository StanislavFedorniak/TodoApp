import { computed, inject, Service, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { AuthResponseDto, LoginDto, RegisterDto } from '../models/auth.model';
import { API_URL } from '../tokens/api-url.token';

const STORAGE_KEY = 'auth_session';

@Service()
export class AuthService {
    private readonly http = inject(HttpClient);
    private readonly endpoint = `${inject(API_URL)}/auth`;

    private readonly session = signal<AuthResponseDto | null>(this.readStoredSession());

    readonly token = computed(() => this.session()?.token ?? null);
    readonly isAuthenticated = computed(() => this.session() !== null);

    login(dto: LoginDto): Observable<AuthResponseDto> {
        return this.http
            .post<AuthResponseDto>(`${this.endpoint}/login`, dto)
            .pipe(tap(response => this.startSession(response)));
    }

    register(dto: RegisterDto): Observable<AuthResponseDto> {
        return this.http
            .post<AuthResponseDto>(`${this.endpoint}/register`, dto)
            .pipe(tap(response => this.startSession(response)));
    }

    logout(): void {
        this.session.set(null);
        try {
            localStorage.removeItem(STORAGE_KEY);
        } catch {

        }
    }

    private startSession(response: AuthResponseDto): void {
        this.session.set(response);
        try {
            localStorage.setItem(STORAGE_KEY, JSON.stringify(response))
        } catch {

        }
    }

    private readStoredSession(): AuthResponseDto | null {
        try {
            const raw = localStorage.getItem(STORAGE_KEY);
            if (!raw) {
                return null;
            }
            const stored = JSON.parse(raw) as Partial<AuthResponseDto>;
            const isValid = 
                typeof stored.token === 'string' &&
                typeof stored.expiresAt === 'string' &&
                Date.parse(stored.expiresAt) > Date.now();
            return isValid ? (stored as AuthResponseDto) : null;
        } catch {
            return null;
        }
    }
}