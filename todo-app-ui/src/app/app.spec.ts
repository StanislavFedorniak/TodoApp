import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { API_URL } from './core/tokens/api-url.token';

const SESSION_KEY = 'auth_session';

describe('App', () => {
  beforeEach(async () => {
    localStorage.removeItem(SESSION_KEY);

    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: API_URL, useValue: 'http://localhost/api' }
      ]
    })
      .compileComponents();
  });

  afterEach(() => localStorage.removeItem(SESSION_KEY));

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should hide navigation when the user is not signed in', async () => {
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;

    expect(compiled.querySelector('nav')).toBeNull();
  });

  it('should show navigation and a log out button when the user is signed in', async () => {
    localStorage.setItem(
      SESSION_KEY,
      JSON.stringify({ token: 'test-token', expiresAt: new Date(Date.now() + 60_000).toISOString() })
    );

    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const compiled = fixture.nativeElement as HTMLElement;

    expect(compiled.querySelector('nav')).not.toBeNull();
    expect(compiled.querySelector('button')?.textContent).toContain('Log out');
  });
});
