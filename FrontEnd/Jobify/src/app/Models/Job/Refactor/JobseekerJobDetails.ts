 interface JobseekerJobDetails {
  id: number;
  title: string;
  description: string;
  location: string;
  minSalary: number;
  maxSalary: number;
  employmentType: string;
  experienceLevel: string;
  createdAt: string;
  postedAge: string;

  companyId: number;
  companyName: string;
  companyLogo?: string | null;
  companyWebsite: string;

  hasApplied: boolean;
  applicationId?: number | null;
  applicationStatus?: string | null;
  isActive: boolean;
}