import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../../Models/Auth/Register';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthserviceService {

  constructor(private httpclient:HttpClient) { }

    register(data: any): Observable<any> {
    console.log(data)
    return this.httpclient.post(`https://localhost:44309/api/Auth/register`, data,{ responseType: 'text' });
  }
}
