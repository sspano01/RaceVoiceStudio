//***********************************************************************************
//** Scale Functions
//***********************************************************************************
#include <stdio.h>
#include <string.h>
#include <math.h>
#include "defs.h" 

#ifndef PCMODE
	#include "app.h"
#endif

double MotecScaleDouble(unsigned short,long);
double AimScaleDouble(unsigned short,long);
double VboxScaleDouble(unsigned short,long);
long VboxScale(unsigned short,long);
static double VBOXGPS(long,int);
extern struct _settings_ settings;

long VboxScale(unsigned short address, long value)
{
    long svalue=value;
    double dvalue;
    switch(address)
    {
        case SCALE_MPH:
            dvalue=(double)value*(double)(0.01); // this is is knots
            dvalue*=(double)1.15078;
            svalue=(long)dvalue; // now mph
            break;
        default: break;
    }

 return svalue;

}


// take the local GPS signal which presents as a double such as
//4227.1808 and convert it to decimal degrees for the map files
//eg. 07717.3644 E is the DDDMM.MMMM format
//
//077 --> degrees
//17 --> minutes
//.3644 --> minutes equals to sec/60
//decimal = degrees + minutes/60 
//
//decimal = 77 + (17.3644 / 60)  
//
//decimal = 77.28941

double GpsEncodingToDegrees( char* gpsencoding )
{
    double a = strtod( gpsencoding, 0 ) ;
    double d = (int)a / 100 ;
    a -= d * 100 ;
    return d + (a / 60) ;
}


double LocalGPS(char* gps, char dir)
{
    double vgps;
    
    vgps=GpsEncodingToDegrees(gps);
    if (dir=='W' || dir=='S') vgps*=(double)(-1);
    return vgps;
}



//Position, Latitude * 100,000 (311924579 = 51 Degrees, 59.24579 Minutes North). This is a true 32 bit signed integer, North being positive.
        //Position, Longitude * 100,000 (11882246 = 0 Degrees, 58.82246 Minutes West). This is a true 32 bit signed integer, West being positive. 

        //  Latitude = Latitude(mins) * 100,000 (311924579 = 3119.24579 Minutes North). Latitude highest bit indicates north/south hemisphere. 0=north, 1=south, Bit 7 in Status is also set.
        //Divide the minutes by 60 to get degrees - 3119.24579/60 = 51.98742983 degrees.
        //Then subtract the round degree number from this result and multiply the remainder by 60 to get the minutes 0.98742983 * 60 = 59.24579
        //So combining the two degrees and minutes results gives 51 degrees 59.24579 minutes
        //Longitude = Longitude(Mins) * 100,000 (5882246 = 58.82246 Minutes West). Longitude highest bit indicates east/west of Greenwich meridian. 0=west,1=east.Bit 6 in Status is also set.
        //In this case the number of minutes is less than 60 so there is no need to divide by 60 to get a degrees component.
        //0 degrees 58.82246 minutes

        // silverstone example
        // cabus 3123842976 x 6097848600
        //52°3.842976 N,1°0.978486 W -->
        //+3123.84297600 +0060.97848600
        // 52.06405 lattitude x -1.016307 longitude
            //0.000000 1 301       Rx d 08  08 00 ed 12 [12 9e 9a 8f] --> 312384143
            //0.000232 1 302       Rx d 08  [00 5d 0c 68] 1a a3 53 f9 --> 6098024



double VBOXGPS(long vgps,int lng)
        {
            double mins = 0;
            long val = 0;

            val = vgps & 0x7fffffff;
            if (lng==1)
            {
                if ((vgps & 0x80000000) ==0) val = val * -1;
            }
            else
            {
                if ((vgps & 0x80000000) != 0) val = val * -1;
            }
            mins = (double)val;
            mins = mins / 100000;
            mins = mins / 60;

            //Console.WriteLine("Minutes=" + mins);
        
            return mins;

        }
        
double VboxScaleDouble(unsigned short address, long value)
{
    double svalue=0;
    char tv[64];

    svalue=(double)(value);
//https://racelogic.support/01VBOX_Automotive/01General_Information/Knowledge_Base/VBOX_Latitude_and_Longitude_Calculations
    switch (address)
    {
        //Position, Latitude * 100,000 (311924579 = 51 Degrees, 59.24579 Minutes North). This is a true 32 bit signed integer, North being positive.
        //Position, Longitude * 100,000 (11882246 = 0 Degrees, 58.82246 Minutes West). This is a true 32 bit signed integer, West being positive. 
        case VBOX_LAT:
                    svalue=VBOXGPS(value,0);
                    break;
        case VBOX_LNG_VEL:
                    svalue=VBOXGPS(value,1);
                    break;
        case VBOX_GFORCE:
                    svalue=(double)(abs(value));
                    svalue=svalue/(double)100;
                    break;
        case SCALE_LAPTIME:
            // each tick is 0.001 seconds
            svalue=svalue/(double)1000;
            break;
            
        default: break;
    }
    return svalue;    
}

// scaling for MOTEC Dash Values
double MotecScaleDouble(unsigned short address, long value)
{
    double svalue=0;
    char tv[64];

    svalue=(double)(value);

 switch(address)
    {
        // each tick is 1/10th of a million degrees
        case SCALE_GPS:
             svalue=svalue*(double)(0.0000001);
            break;

            // each tick is 1/100th of a volt
     case SCALE_VOLTS:
         svalue=svalue*(double)(0.01);
         break;

        case CAN_LAT_G:
            // each tick is 0.01 seconds
            svalue=(double)(abs(value));
            svalue=svalue/(double)100;
            break;
        case SCALE_LAT_G_RAW:
            svalue=(double)(value);
            svalue=svalue/(double)100;
            break;

        case SCALE_LAPTIME:
            // each tick is 0.01 seconds
            svalue=svalue/(double)100;
            break;

            break;

     default: break;
 }
    //sprintf(tv,"a=%d lv=%d rf=%f\r\n",address,value,svalue);
    //DBG(tv);
 return svalue;

}

long MotecScale(unsigned short address, long value)
{
    long svalue=value;
 switch(address)
    {
         // motec default is 1m per tick
         case SCALE_DISTANCE:
             svalue=(long)((float)value*3.28); // meters to feet
             break;

        case SCALE_TPS:
            // default is 0.1 degrees
            svalue=value/10;
            break;

        case SCALE_TEMP:
            // default is 0.1 degrees CELCIUS
            svalue=value/10;
            svalue=(long)((float)svalue * (float)9/(float)5);
            svalue+=32; // to degree F
            break;

        case SCALE_OIL:
            // default is 0.1 kpa
            svalue=value/10;
            svalue=(long)((double)svalue*(double)0.145);
            break;

        case SCALE_BRAKE:
            // default is 1 kpa
            svalue=(long)((double)value*(double)0.145);
            break;

         //motec is 0.1km/h per tick
        case SCALE_MPH:
            svalue=value/10; // now 1km per tick
            svalue=(long)((double)svalue*(double)0.6214);  // now miles per hour
            break;

       case SCALE_RPM:
           // default is 0.1hz
           // 1hz = 60 rpm
           value=value/10;
           svalue=value*60;
           break;
     default: break;
 }

 return svalue;

}

// decimal time represented as
// so 2.756 decimal minutes is sent as 2756
double dec_to_ms(unsigned long decimal)
{
    double dms=0;
    decimal&=0xffff;
    if (decimal & 0x8000) decimal|=0xffff0000; // sign extend
    dms=(double)((long)decimal);
    
    dms=dms/(double)1000;
    return dms;
    
}
// scaling for AIM MXL2 Dash Values
double AimScaleDouble(unsigned short address, long value)
{
    double svalue=0;
    svalue=(double)(value);
 switch(address)
    {
     case INTERNAL_LAP_TIMER:
             svalue=svalue*(double)(0.0000064);
            break;
         
         // each tick is 1/10th of a million degrees
      case SCALE_GPS:
             svalue=svalue*(double)(0.0000001);
            break;

     // each tick is 1/10th of a volt
     case SCALE_VOLTS:
         svalue=svalue*(double)(0.1);
         break;


        case CAN_LAT_G:
            // units are 0.01g per tick
            svalue=(double)(abs(value));
            svalue=svalue/(double)100;
            break;
            
        case SCALE_LAT_G_RAW:
            svalue=(double)(value);
            svalue=svalue/(double)100;
            break;

        case SCALE_LAPTIME:
            if (settings.dash_type==AUTOSPORTS)
            {
                // native value is in decimal minutes
                // so 2756 would be 2.756 decimal minutes
                // we need to convert that direcly to 1ms steps
                // https://www.calculatorsoup.com/calculators/time/decimal-to-time-calculator.php
                svalue=dec_to_ms((unsigned long)value);
                
            }
            else
            {
                // units are 1ms per tick
                svalue=svalue/(double)1000;
            }
            break;

     default: break;
 }

 return svalue;

}

long AimScale(unsigned short address, long value)
{
    long svalue=value;
    switch(address)
    {
        case SCALE_BAR_TO_PSI:
            // 1 bar is 14.5 PSI
            svalue=(long)((double)value*(double)14.5);
            break;
            
        case SCALE_TPS:
            // default is  1 degree per tick
            break;

        case SCALE_TEMP:
            // default is 1 degree per tick in degrees F
            break;

        case SCALE_TEMP_C:
            svalue=value/10;
            svalue=(long)((float)svalue * (float)9/(float)5);
            svalue+=32; // to degree F
            break;

        case SCALE_OIL:
            // default is 1psi per tick
            break;

        case SCALE_BRAKE:
            // default is 1psi per tick
            break;

        case SCALE_MPH_D10_MM:
            // default is 0.1 mph per tick
            svalue=svalue/10;
            break;

        case SCALE_MPH_D10_KM:
          
            svalue=value/10; // now 1km per tick
            svalue=(long)((double)svalue*(double)0.6214);  // now miles per hour
            break;
            
        case SCALE_MPH:
            // default is 1 mph per tick
            break;

       case SCALE_RPM:
           // default is 1rpm per tick
           break;
     default: break;
    }

 return svalue;

}