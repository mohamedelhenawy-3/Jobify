export interface CompanyDto {
  id: number;
  companyName: string;
  companyWebsite?: string;
  description?: string;
  industry?: string;
  location?: string;
  companySize?: string;
  founded?: string;
  logoUrl?: string;
  createdAt: string;

  // ===== STATS =====
  totalJobs: number;
  activeJobs: number;
}