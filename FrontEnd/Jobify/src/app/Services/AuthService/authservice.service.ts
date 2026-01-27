import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Register } from '../../Models/Auth/Register';
import { Observable, tap } from 'rxjs';
import { login } from '../../Models/Auth/login';
import { DeviceserviceService } from '../DeviceService/deviceservice.service';
import { loginresponse } from '../../Models/Auth/LoginResponse';
import { refreshtoken } from '../../Models/Auth/refreshreq';

@Injectable({
  providedIn: 'root'
})
export class AuthserviceService {
private accessTokenKey = 'access_token';
private refreshTokenKey = 'refresh_token';
  constructor(private httpclient:HttpClient,private deviceservice:DeviceserviceService) { }

    register(data: any): Observable<any> {
    console.log(data)
    return this.httpclient.post(`https://localhost:44309/api/Auth/register`, data,{ responseType: 'text' });
    }
    getAccessToken(): string | null {
      return localStorage.getItem(this.accessTokenKey);
       }

    getRefreshToken(): string | null {
       return localStorage.getItem(this.refreshTokenKey);
    }
      setToken(access_token:string,refresh_token:string){
       localStorage.setItem(this.accessTokenKey, access_token);
       localStorage.setItem(this.refreshTokenKey,refresh_token);
      }
      login(data: login): Observable<loginresponse> {

        var deviceId=this.deviceservice.getdeviceId();
        return this.httpclient.post<loginresponse>(
       `https://localhost:44309/api/Auth/login`,
         { ...data, deviceId }
         ).pipe(
         tap(response => {
          this.setToken(response.accessToken, response.refreshToken);
         })
        );
      }
private clearSession() {
  localStorage.removeItem(this.accessTokenKey);
  localStorage.removeItem(this.refreshTokenKey);
}


refreshToken(): Observable<loginresponse> {
  return this.httpclient.post<loginresponse>(
    `https://localhost:44309/api/Auth/refresh`,
    {
      refreshToken: this.getRefreshToken(),
      deviceId: this.deviceservice.getdeviceId()
    }
  );
}


  logout(): void {
  const refreshToken = this.getRefreshToken();
  const deviceId = this.deviceservice.getdeviceId();
  if (refreshToken) {
    this.httpclient.post(
      `https://localhost:44309/api/Auth/logout`,
      { refreshToken, deviceId }
    ).subscribe({
      next: () => this.clearSession(),
      error: () => this.clearSession() // even if backend fails
    });
  } else {
    this.clearSession();
  }
  }

}

