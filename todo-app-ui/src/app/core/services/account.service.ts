import { inject, Service } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountDeleteDto } from '../models/account.model';
import { API_URL } from '../tokens/api-url.token';

@Service()
export class AccountService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = `${inject(API_URL)}/account`;

  delete(dto: AccountDeleteDto): Observable<void> {
    return this.http.post<void>(`${this.endpoint}/delete`, dto);
  }
}
