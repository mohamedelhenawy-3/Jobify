export interface SavedJob {
  id: number;
  logo: string | null;
  title: string;
  location: string | null;
  jobType: number | string;
  experienceLevel: number | string;
  savedAt: string;
}