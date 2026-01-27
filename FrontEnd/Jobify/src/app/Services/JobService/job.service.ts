import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { industrieswithjobcount } from '../../Models/Industries/industrieswithjobcount';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class JobService {

   constructor(private HttpClient:HttpClient) { }



  GetAllIndustriesWithJobCount():Observable<industrieswithjobcount[]>{
    return this.HttpClient.get<industrieswithjobcount[]>(`https://localhost:44309/api/Job/GetAllIndustriesWithJobCount`)
  }
}
