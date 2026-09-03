import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { PagedResult, PaginationQuery } from '../models/pagination.model';
import { CreateTeacher, Teacher, UpdateTeacher } from '../models/teacher.model';

@Injectable({ providedIn: 'root' })
export class TeacherService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/teachers';

  getPaged(query: PaginationQuery): Observable<PagedResult<Teacher>> {
    let params = new HttpParams()
      .set('page', query.page)
      .set('pageSize', query.pageSize);

    if (query.search?.trim()) {
      params = params.set('search', query.search.trim());
    }

    return this.http.get<PagedResult<Teacher>>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Teacher> {
    return this.http.get<Teacher>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateTeacher): Observable<Teacher> {
    return this.http.post<Teacher>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateTeacher): Observable<Teacher> {
    return this.http.put<Teacher>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}