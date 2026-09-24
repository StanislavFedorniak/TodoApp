import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../tokens/api-url.token';
import {TodoTask, TodoTaskCreateDto, TodoTaskUpdateDto} from '../models/task.model';

@Injectable ({
  providedIn: 'root'
})
export class TodoTaskService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = inject(API_URL);
  private readonly endpoint = `${this.apiUrl}/TodoTask`

  getAll(): Observable<TodoTask[]> {
    return this.http.get<TodoTask[]>(this.endpoint);
  }

  getById(id: string): Observable<TodoTask> {
    return this.http.get<TodoTask>(`${this.endpoint}/${id}`);
  }

  create(dto: TodoTaskCreateDto): Observable<TodoTask> {
    return this.http.post<TodoTask>(`${this.endpoint}`, dto);
  }

  update(id: string, dto: TodoTaskUpdateDto): Observable<void> {
    return this.http.put<void>(`${this.endpoint}/${id}`, dto);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.endpoint}/${id}`);
  }
}
