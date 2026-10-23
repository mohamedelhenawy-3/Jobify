export type JoinStatus = 'Pending' | 'Approved' | 'Rejected';

export interface JoinRequestItem {
  id: number;
  fullName: string;
  logo:string;
  email: string;
  createdAt: string;
  respondedAt?: string | null;
  status: JoinStatus;
}

export interface JoinRequestFilter {
  status: JoinStatus;
  pageNumber: number;
  pageSize: number;
}