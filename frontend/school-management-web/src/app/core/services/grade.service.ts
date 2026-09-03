import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CreateGrade, Grade, UpdateGrade } from '../models/grade.model';
import { PagedResult, PaginationQuery } from '../models/pagination.model';

@Injectable({ providedIn: 'root' })
export class GradeService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/grades';

  getPaged(query: PaginationQuery): Observable<PagedResult<Grade>> {
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

    return this.http.get<PagedResult<Grade>>(this.baseUrl, { params });
  }

  getById(id: number): Observable<Grade> {
    return this.http.get<Grade>(`${this.baseUrl}/${id}`);
  }

  create(payload: CreateGrade): Observable<Grade> {
    return this.http.post<Grade>(this.baseUrl, payload);
  }

  update(id: number, payload: UpdateGrade): Observable<Grade> {
    return this.http.put<Grade>(`${this.baseUrl}/${id}`, payload);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}