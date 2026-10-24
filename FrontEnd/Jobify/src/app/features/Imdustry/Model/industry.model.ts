export interface Industry {
  id: number;
  name: string;
  slug: string;
  isActive: boolean;
}

export interface PopularIndustry {
  id: number;
  name: string;
  slug: string;
  jobsCount: number;
}