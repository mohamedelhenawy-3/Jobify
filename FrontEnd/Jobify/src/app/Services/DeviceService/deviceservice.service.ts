import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DeviceserviceService {
  private deviceKey="deviceid";
  constructor() { }


  getdeviceId() :string{
  let deviceid=localStorage.getItem(this.deviceKey);
  if(!deviceid){
    deviceid=crypto.randomUUID();
    localStorage.setItem("deviceId",deviceid);
  }
  return deviceid;
  }
}
