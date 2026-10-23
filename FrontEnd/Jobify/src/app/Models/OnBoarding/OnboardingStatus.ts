import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';

export interface EmployerOnboardingStatus {
  hasCompany: boolean;
  companyId?: number;
  companyName?: string;
  verificationStatus: 'None' | 'Pending' | 'Approved' | 'Rejected';
  isVerified: boolean;
  hasPendingJoinRequest: boolean;
  pendingCompanyId?: number;
}