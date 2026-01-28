import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Industry } from '../../Models/Industry/industry';
import { Observable } from 'rxjs';
import { EmploymentType } from '../../Models/EmploymentType/EmploymentType ';
import { WorkMode } from '../../Models/WorkMode/WorkMode';

@Injectable({
  providedIn: 'root'
})
export class LookupService {

    constructor(private http: HttpClient) {}

  getIndustries(): Observable<Industry[]> {
    return this.http.get<Industry[]>('https://localhost:44309/api/LooksUp/Industries');
  }

  getEmploymentTypes(): Observable<EmploymentType[]> {
    return this.http.get<EmploymentType[]>('https://localhost:44309/api/LooksUp/EmploymentTypes');
  }

  getWorkModes(): Observable<WorkMode[]> {
    return this.http.get<WorkMode[]>('https://localhost:44309/api/LooksUp/WorkModes');
  }
}
