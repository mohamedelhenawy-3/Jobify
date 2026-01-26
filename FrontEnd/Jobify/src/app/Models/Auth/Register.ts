export interface Register {
  email: string;
  password: string;
  role: 'JobSeeker' | 'Employer';

  // JobSeeker
  fullName?: string;
  phoneNumber?: string;

  // Employer
  companyName?: string;
  companyWebsite?: string;
}
