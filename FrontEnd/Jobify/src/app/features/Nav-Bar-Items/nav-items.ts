import { SidebarItem } from "../../Models/SideBar/sidebar";

export interface NavGroup {
  label: string;
  route?: string;            // simple link (no dropdown)
  children?: SidebarItem[];  // dropdown items
}

export const NAV_ITEMS: Record<string, NavGroup[]> = {
  JobSeeker: [
    { label: 'Home', route: '/Jobify/home' },
    { label: 'Jobs', children: [
      { label: 'Find Jobs',  route: '/Jobify/jobs',        icon: 'fas fa-search' },
      { label: 'Saved Jobs', route: '/Jobify/SavedJobs',   icon: 'fas fa-bookmark' },
      { label: 'Alert Jobs', route: '/Jobify/AlertJobs',   icon: 'fas fa-bell' },
    ]},
    { label: 'My Applications', route: '/Jobify/seeker-applications' },
    { label: 'Account', children: [
      { label: 'Profile',         route: '/Jobify/ownseekerprofile', icon: 'fas fa-user' },
      { label: 'Company Profile', route: '/Jobify/mycompany',        icon: 'fas fa-building' },
    ]},
  ],
  Employer: [
    { label: 'Dashboard', route: '/Jobify/employerdashboard' },
    { label: 'Jobs', children: [
      { label: 'Find Jobs', route: '/Jobify/jobs',          icon: 'fas fa-search' },
      { label: 'My Jobs',   route: '/Jobify/employerjobs',  icon: 'fas fa-briefcase' },
    ]},
    { label: 'Account', children: [
      { label: 'Company Profile', route: '/Jobify/mycompany',         icon: 'fas fa-building' },
      { label: 'Profile',         route: '/Jobify/employer/profile',  icon: 'fas fa-user' },
    ]},
  ],
};