export interface JobRecommendation {
  jobId: number;
  title: string;
  companyName: string;
  matchScore: number;
  location?: string;
  minSalary?: number;
  maxSalary?: number;
  workMode?: string;
}