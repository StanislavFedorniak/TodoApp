import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category, CategoryCreateDto, CategoryUpdateDto } from '../models/category.model';
import { API_URL } from '../tokens/api-url.token';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(API_URL);

  private readonly endpoint = `${this.apiUrl}/Category`;

  getAll(): Observable<Category[]> {
    return this.http.get<Category[]>(this.endpoint);
  }

  getById(id: string): Observable<Category> {
    return this.http.get<Category>(`${this.endpoint}/${id}`);
  }

  create(dto: CategoryCreateDto): Observable<Category> {
    return this.http.post<Category>(this.endpoint, dto);
  }

  update(id: string, dto: CategoryUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.endpoint}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}
