import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';


// 🔹 Request DTO
export interface GlobalSearchDto {
  keyword?: string;
  type: 'all' | 'job' | 'user' | 'company';

  industryIds?: number[];
  workModeIds?: number[];
  employmentTypeIds?: number[];
  postedWithinDays?: number;

  industry?: string;
  companySize?: string;

  location?: string;
}


// 🔹 Response DTO
export interface GlobalSearchResponse {
  jobs: any[];
  users: any[];
  companies: any[];
}


@Injectable({
  providedIn: 'root'
})
export class SearchsystemService {

  private baseUrl = 'https://localhost:44309/api/Search'; // ⚠️ change if needed

  constructor(private http: HttpClient) {}
    search(dto: GlobalSearchDto): Observable<GlobalSearchResponse> {

    let params = new HttpParams();

    Object.keys(dto).forEach((key) => {
      const value = dto[key as keyof GlobalSearchDto];

      if (
        value !== null &&
        value !== undefined &&
        value !== '' &&
        !(Array.isArray(value) && value.length === 0)
      ) {
        // 🔥 Handle arrays (VERY IMPORTANT)
        if (Array.isArray(value)) {
          value.forEach(v => {
            params = params.append(key, v.toString());
          });
        } else {
          params = params.set(key, value.toString());
        }
      }
    });

    return this.http.get<GlobalSearchResponse>(this.baseUrl, { params });
  }
  }
