import { HttpClient } from '@angular/common/http';
import {
  Inject,
  Injectable,
  PLATFORM_ID
} from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

import { Register } from '../../Models/Auth/Register';
import { login } from '../../Models/Auth/login';
import { loginresponse } from '../../Models/Auth/LoginResponse';
import { refreshtoken } from '../../Models/Auth/refreshreq';
import { DeviceserviceService } from '../DeviceService/deviceservice.service';

export interface JwtPayload {
  exp: number;
  [key: string]: any;
}

@Injectable({
  providedIn: 'root'
})
export class AuthserviceService {

  private accessTokenKey = 'access_token';
  private refreshTokenKey = 'refresh_token';

  private loggedIn = new BehaviorSubject<boolean>(false);
  public isLoggedIn$ = this.loggedIn.asObservable();

  constructor(
    private httpclient: HttpClient,
    private deviceservice: DeviceserviceService,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    if (isPlatformBrowser(this.platformId)) {
      this.loggedIn.next(this.checkIsLoggedIn());
    }
  }

  /* ===================== AUTH STATE ===================== */

  private checkIsLoggedIn(): boolean {
    const payload = this.getPayload();
    if (!payload) return false;

    const currentTime = Math.floor(Date.now() / 1000);
    return payload.exp > currentTime;
  }

  isLoggedIn(): boolean {
    return this.checkIsLoggedIn();
  }

  /* ===================== JWT ===================== */

  getPayload(): JwtPayload | null {
    const token = this.getAccessToken();
    if (!token) return null;

    try {
      return jwtDecode<JwtPayload>(token);
    } catch (error) {
      console.error('Invalid token', error);
      return null;
    }
  }

  getUserRole(): string | null {
    const payload = this.getPayload();
    if (!payload) return null;

    const roleClaim =
      'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

    return payload[roleClaim] ?? null;
  }

  /* ===================== TOKEN STORAGE ===================== */

  getAccessToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem(this.accessTokenKey);
  }

  getRefreshToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem(this.refreshTokenKey);
  }

  setToken(accessToken: string, refreshToken: string): void {
    if (!isPlatformBrowser(this.platformId)) return;

    localStorage.setItem(this.accessTokenKey, accessToken);
    localStorage.setItem(this.refreshTokenKey, refreshToken);
    this.loggedIn.next(true);
  }

  private clearSession(): void {
    if (!isPlatformBrowser(this.platformId)) return;

    localStorage.removeItem(this.accessTokenKey);
    localStorage.removeItem(this.refreshTokenKey);
    this.loggedIn.next(false);
  }

  /* ===================== API CALLS ===================== */

  register(data: Register): Observable<any> {
    return this.httpclient.post(
      `https://localhost:7026/api/Auth/register`,
      data,
      { responseType: 'text' }
    );
  }

  login(data: login): Observable<loginresponse> {
    const deviceId = this.deviceservice.getdeviceId();

    return this.httpclient
      .post<loginresponse>(
        `https://localhost:7026/api/Auth/login`,
        { ...data, deviceId }
      )
      .pipe(
        tap(response => {
          this.setToken(response.accessToken, response.refreshToken);
        })
      );
  }

  refreshToken(): Observable<loginresponse> {
    return this.httpclient.post<loginresponse>(
      `https://localhost:7026/api/Auth/refresh`,
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
      this.httpclient
        .post(
          `https://localhost:7026/api/Auth/logout`,
          { refreshToken, deviceId }
        )
        .subscribe({
          next: () => this.clearSession(),
          error: () => this.clearSession()
        });
    } else {
      this.clearSession();
    }
  }
}
