import { Component, OnInit } from '@angular/core';
import { industrieswithjobcount } from '../../Models/Industries/industrieswithjobcount';
import { JobService } from '../../Services/JobService/job.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css'
})
export class HomeComponent implements OnInit  {

 indstries:industrieswithjobcount[]=[];
   // map each industry to an icon path
  icons: { [key: string]: string } = {
    'IT & Software': 'assets/industry/software-application.png',
    'Finance': 'assets/industry/salary.png',
    'Healthcare': 'assets/industry/hospitality.png',
    'Education': 'assets/industry/education.png',
    'Marketing': 'assets/industry/content-strategy.png',
    'Construction': 'assets/industry/fast-delivery.png',
    'Retail': 'assets/industry/online-retailer.png',
    'Manufacturing': 'assets/industry/healthcare.png',
    'Transportation': 'assets/industry/shovel.png',
    'Hospitality': 'assets/industry/hospitality.png'
  };
 constructor(private api:JobService){

 }
 ngOnInit(): void {
 this.api.GetAllIndustriesWithJobCount().subscribe({
  next:(data)=>{
    console.log(data);
      this.indstries =data;
  }
 })

}
  getIcon(industryName: string): string {
    return this.icons[industryName] || 'assets/icons/default.svg';
  }


}
