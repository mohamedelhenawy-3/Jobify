export interface Register {
  email: string;
  password: string;
  confirmPassword:string;
  role: 'JobSeeker' | 'Employer';
  fullName?: string;
}
