export interface jobdetails {
  id: number;
  title: string;
  logo?: string | null;
  description: string;
  location: string;
  minSalary: number;
  maxSalary: number;
  employmentType: string;
  exprianceLevel: string;
  requirements: string;
  responsibilities:string;
  status: string;
  createdAt: string;
  postedAge: string;
  companyName: string;
  companyAge?: string;
  totalApplications: number;
  postedByEmployerId?: string;
  postedByName?: string;
  isOwnerView: boolean;
}
