export interface JobRecommendation {
  jobId: number;
  title: string;
  logo:string;
  companyName: string;
  matchScore: number;
  location: string;
  minSalary: number;
  maxSalary: number;
  workModeId:number;
  industryId:number;
  employmentTypeId:number;
  statusId:number;
}