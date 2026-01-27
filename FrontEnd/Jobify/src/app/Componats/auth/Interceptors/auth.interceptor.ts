import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { AuthserviceService } from '../../../Services/AuthService/authservice.service';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthserviceService);

  if (req.url.includes('/Auth/refresh')) {
    return next(req);
  }

  const accesstoken=authService.getAccessToken();

  const authReq = accesstoken
    ? req.clone({
        setHeaders: {
          Authorization: `Bearer ${accesstoken}`
        }
      })
    : req;
   return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        return authService.refreshToken().pipe(
          switchMap(tokens => {
            authService.setToken(tokens.accessToken, tokens.refreshToken);

            return next(
              req.clone({
                setHeaders: {
                  Authorization: `Bearer ${tokens.accessToken}`
                }
              })
            );
          }),
          catchError(err => {
            authService.logout();
            return throwError(() => err);
          })
        );
      }

      return throwError(() => error);
    })
  );
};