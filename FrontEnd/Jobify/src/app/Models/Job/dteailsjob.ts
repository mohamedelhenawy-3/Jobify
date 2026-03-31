export interface jobdetails {
  id:number;
  title: string;
  logo?:string;
  description: string;
  location: string;
  minSalary: number;
  maxSalary: number;
  workMode: string;
  industry: string;
  employmentType: string;
    responsibilities:string;
  educationOrExperience:string;
  requirements:string;
  status:string;
    isSaved?: boolean;
}
