import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult, PaginationQuery } from '../models/pagination.model';
import { CreateStudent, Student, UpdateStudent } from '../models/student.model';

@Injectable({ providedIn: 'root' })
export class StudentService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/students';

  getPaged(query: PaginationQuery): Observable<PagedResult<Student>> {
  let params = new HttpParams()
    .set('page', query.page)
    .set('pageSize', query.pageSize);

  if (query.search?.trim()) {
    params = params.set('search', query.search.trim());
  }

  if (query.sortBy) {
    params = params.set('sortBy', query.sortBy);
    params = params.set('sortDirection', query.sortDirection ?? 'asc');
  }

  return this.http.get<PagedResult<Student>>(this.baseUrl, { params });
}

  getById(id: number): Observable<Student> {
    return this.http.get<Student>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateStudent): Observable<Student> {
    return this.http.post<Student>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateStudent): Observable<Student> {
    return this.http.put<Student>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}