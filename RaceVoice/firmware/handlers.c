//***********************************************************************************
//** Lap Data Handlers
//***********************************************************************************
#include <stdio.h>
#include <string.h>
#include <math.h>
#include "defs.h"

#ifndef PCMODE
	#include "app.h"
#endif

extern struct _global_ global;
extern struct _settings_ settings;
extern void DBG(char*);
extern void Announce(int);
extern void LapTimer(int,int);
extern void SpeechByte(unsigned char);
extern int SpeechString(char*,int,int); 
extern int Speak(char*,int);

void ShiftTone(void);


static int startup_done=0; 

void HandleMPH(int inlateral)
{
    int index=0;
    int speak=0;
                if (global.last_mph!=global.mph)
                {
                    if (global.mph>=settings.mph && settings.mph!=0 && inlateral==0) speak=1;
                    
					/*
					for (index=0;index<4;index+=2)
                    {
                        if (global.mph>=settings.mph_range[index] && global.mph<=settings.mph_range[index+1]) speak=1;
                    }
					*/

                    if (global.segment_mph || (speak && settings.mph_announce)) Announce(SPEED);
#ifdef BETA
#ifdef JIM
                            for (index=0;index<MPH_TRIGGERS;index++)
                            {
                                if (settings.mph_trigger[index]!=0 && global.tps>settings.tps_high)
                                {
                                    if ( (global.mph>=(settings.mph_trigger[index]-settings.mph_trigger_delta)) && (global.mph<=settings.mph_trigger[index]))
                                    {
                                            Announce(SPEED); 
                                    }
                                }
                            }
#endif
#endif
                    }
                    global.last_mph=global.mph;
              
}

void HandleWheelSpeed(void)
{
    if (global.wheel_locked)
    {
        if (global.tps>=settings.tps_high)
        {
            switch(global.wheel_locked)
            {
                case 1: Announce(TIRELOCK_FRONT); break;
                case 2: Announce(TIRELOCK_REAR); break;
                case 3: Announce(TIRELOCK); break;
                default: break;
            }
            global.wheel_locked=0;
        }
    }
}

int CalcRatio(int ileft, int iright)
{
    double left=(double)ileft;
    double right=(double)iright;
    double ratio;
    int iratio;
           if (left<=right)
           {
               ratio=right/left;
           }
           else
           {
               ratio=left/right;
           }
		   ratio=ratio*(double)100; // for percent
           iratio=(int)(ratio-(double)100); 
    return iratio;
}

void HandleWheelSpeedIRQ(void)
{
    int minf=10000;
    int minr=10000;
    int psi_threshold=0;
    int delta_pct=0;

    // convert index to PSI
    psi_threshold=settings.wheel_speed_brake_threshold * 100;
    delta_pct=settings.wheel_speed_delta+1; // index is zero based where 0 index is 1%
    if (settings.wheel_speed_enable && global.wheel_locked==0)
    {
       if(global.brake_front_psi>=psi_threshold)
       {
	
           // find the min speeds
           if (global.wheel_speed[0]<minf) minf=global.wheel_speed[0];
           if (global.wheel_speed[1]<minf) minf=global.wheel_speed[1];

           if (global.wheel_speed[2]<minr) minr=global.wheel_speed[2];
           if (global.wheel_speed[3]<minr) minr=global.wheel_speed[3];
           
          // difference between fronts
          if (CalcRatio(global.wheel_speed[0],global.wheel_speed[1])>=delta_pct) 
          {
              global.wheel_locked=1;
              return;
          }
           
          // rears
          if (CalcRatio(global.wheel_speed[2],global.wheel_speed[3])>=delta_pct)
          {
                global.wheel_locked=2;
                return;
          }
           
         // front to back? we are really sliding now
          if (CalcRatio(minf,minr)>=delta_pct)
          {
              global.wheel_locked=3; // make sure we announce something!
              if (minf>minr) global.wheel_locked=2; // back is locked 
              if (minr>minf) global.wheel_locked=1; // front is locked 
              return;
          }
           
       }
    }
}

void HandleLinear(void)
{
        if (global.linear_g_triggered!=0)
        {
            if (settings.linear_gforce_announce) Announce(LINEAR);
            global.linear_g_triggered=0;
        }
}



static int gps_stage = 0;
static int minimum_latched = 0;
static int split_latched = 0;
static int entry_latched = 0;
static int exit_latched = 0;
static int turn_in_latched = 0;
static int max_linear_latched = 0;
static int max_lateral_latched = 0;
static int split_was_latched[MAX_SEGMENTS + 2];


int HandleLateral(void)
{
    //char im[64];
	char msg[32];
	static int lat_state = 0;

    if (global.rolling_lateral_g==1)
    {
		lat_state = 0;
        if (global.lateral_g>=(double)(0.1))  Announce(LATERAL);
        return 1;
    }
#ifdef PCMODE
	sprintf(msg, "LatState=%d\r\n", lat_state);
	//DBG(msg);
#endif

	switch (lat_state)
	{
		case 0:
			if (global.lateral_g >= settings.lateral_g_high_trigger)
			{
				lat_state = 1;
			}
			break;
		case 1:
			if (global.lateral_g < settings.lateral_g_high_trigger)
			{
				lat_state=2;
				break;
			}
			if (minimum_latched || exit_latched || max_lateral_latched)
			{
				lat_state = 2;
				break;
			}
			if (settings.lateral_gforce_announce && turn_in_latched==0)
			{
				Announce(LATERAL);
				return 1;
			}
			break;
		case 2: if (minimum_latched == 0 && exit_latched == 0 && max_lateral_latched == 0)
				{
					lat_state = 3;
				}
				break;
		case 3:
			if (gps_stage == 0)
			{
				lat_state = 0;
			}
			break;

		default:lat_state = 0; break;
	}
      
      return 0;
}


void HandleRPM(void)
{
	static int up_rpm=0;
	static int down_rpm=0;
// global handlers
    if (global.overrev!=0)
    {
        if (settings.overrev_announce)
        {
            Announce(OVERREV);
        }
        global.overrev=0;
    }
        // lets at least make sure we are moving before we start telling the driver to shift
        if (global.rpm>=settings.rpm_high)
		{
            
            // upshift is enabled
            if (settings.upshift_announce & 1)
            {
                if (settings.upshift_announce & 2)
                {
                    ShiftTone();
                }
                else
                {
                    // speech
					if (up_rpm<settings.rpm_notice && global.tps>settings.tps_high)
					{
						Announce(UP);
					}
                    up_rpm++;
                }
            }
		}
    
		if (global.rpm<=(settings.rpm_high-500))
		{
			up_rpm=0;
		}

        if (global.rpm<=settings.rpm_low) 
		{
			if (down_rpm<settings.rpm_notice && settings.downshift_announce && global.tps>settings.tps_high) Announce(DOWN);
			down_rpm++;
		}
		else
		{
			down_rpm=0;
		}
    
    

}
 

void ClearLapData(void)
{
      global.lap_time=0;
      global.last_lap_time=0;
      global.best_lap_time=0;

}
#ifdef AUDIO_LOOP
// give the user some vitals on startup or after timeout
void HandleStartup(void)
{
        if (global.canrx>=500)
        {
              Announce(OIL);
            //  Announce(TEMP);
             //  Announce(VOLTS);
                global.canrx=0;
    }
} 


#else
// give the user some vitals on startup or after timeout
void HandleStartup(void)
{

    if (startup_done==0)
    {
        if (global.canrx>=800 || global.sys_timer>10)
        {
            if (global.canrx!=0)
            {
                if (settings.dash_type!=STANDALONE)
                {
                    if (settings.dash_type!=VBOX)
                    {
                        if (settings.oil_announce && settings.dash_type!=OBD_II) Announce(OIL);
                        if (settings.temp_announce) Announce(TEMP);
                        if (settings.volts_announce) Announce(VOLTS);
                    }
                }
            }
            startup_done=1;
            
    }
    }
}

#endif


//http://jonisalonen.com/2014/computing-distance-between-coordinates-can-be-simple-and-fast/
//http://www.movable-type.co.uk/scripts/latlong.html
//https://gps-coordinates.org/distance-between-coordinates.php
//Since the distance is relatively small, you can use the equirectangular distance approximation.
//This approximation is faster than using the Haversine formula. 
//So, to get the distance from your reference point (lat1/lon1) to the point your are testing (lat2/lon2), use the formula below. Important Note: you need to convert all lat/lon points to radians:
//R = 6371  // radius of the earth in km
//x = (lon2 - lon1) * cos(0.5*(lat2 + lat1))
//y = lat2 - lat1
//d = R * sqrt(x*x + y * y)
double ToRad(double val)
{
	val = val * (double)3.14159265359 / (double)180;
	return val;
}

int GPSDistance(double lat, double lng,double pos_lat,double pos_lng)
{
	double R, x, y, d;
	int feet = 0;
	//1 km = 3,280.839895 ft
	R = 6371; // radius of earth
	R *= 3280.839895; // now in feet
	x = (ToRad(lng) - ToRad(pos_lng)) * cos(0.5*(ToRad(lat) + ToRad(pos_lat)));
	y = ToRad(lat) - ToRad(pos_lat);
	d = R * sqrt((x*x) + (y*y));
	feet = (int)fabs(d);

	//R = 6371  // radius of the earth in km
	//x = (lon2 - lon1) * cos(0.5*(lat2 + lat1))
	//y = lat2 - lat1
	//d = R * sqrt(x*x + y * y)
	return feet;
}

// in the original version a 0.001 bubble could have been a match anywhere from 250 to 450 feet
// this takes 25uS to compute
int GPSMatch(double lat, double lng, int type)
{
	char txt[60];
	int match = 0;
	int traceit = 0;
	int bubble = 60;
	int feet = 0;
	static float last_lat = 0;
	static float last_lng = 0;
	static float dlat = 0;
	static float dlng = 0;
	static float int_lat = 0;
	static float int_lng = 0;
	float global_lat;
	float global_lng;
	float tlat = 0;
	float tlng = 0;
	static int int_step = 0;
#ifdef GPS_MATCH_TIMING
	BLUE_LEDOn();
#endif
    
   
	// 150mph is 220 feet per second
	// gps error is generally +/- 30 feet
	// smartycam stream is 1hz
	// add in track width as well of another 50 feet
	// and now we are up to near 300 feet
	// so error could be in the range of 300 feet
	switch (settings.gpswindow)
	{
		case 0:bubble = 30; break;
		case 1:bubble = 60;  break;
		case 2:bubble = 90; break;
		case 3:bubble = 120; break;
		case 4: bubble = 220; break;
		default:bubble = 60; break;
	}
	//bubble = bubble * 2; // you could be ahead or behind of the gps trap depending on sample point
	bubble += 30; // for gps position error
//	bubble += 50; // general width of a race track 
	// these are in decimal degrees
// degrees . (minutes/60) + (seconds/3600) 
#ifdef TRACE_GPS
	traceit = TRACE_GPS;
#endif

	if (settings.dash_type == SMARTY)
	{
		// apply interpolation method for low data rate 1hz gps
		// local temps
		global_lat = global.gps_lat;
		global_lng = global.gps_lng;


		// for low sample rate gps, we will get one difference and then alot are the same after it
		tlat = (last_lat - global_lat);
		tlng = (last_lng - global_lng);


		// only update on the differences and skip the first sample which would be large
		if (fabs(tlat) != 0 && fabs(tlng) != 0 && fabs(tlat) < 1 && fabs(tlng) < 1)
		{
			dlat = tlat;
			dlng = tlng;

			int_lng = dlng / (float)GPS_INTEROPLATE_STEPS;
			int_lat = dlat / (float)GPS_INTEROPLATE_STEPS;
			int_step = 0;
		}

		if (fabs(tlat) == 0 && fabs(tlng) == 0 && int_step <= GPS_INTEROPLATE_STEPS)
		{
			int_step++;
			global_lat += int_lat * (float)int_step;
			global_lng += int_lng * (float)int_step;
		}
	}
	else
	{
		// local temps raw at 10hz
		global_lat = global.gps_lat;
		global_lng = global.gps_lng;
	}

	if (traceit)
	{
		if (traceit == CHECKER)
		{
			DBG("Checking GPS for Start-Finish Marker\r\n");
		}

		if (traceit & type)
		{
			sprintf(txt, "NEW Compare %f,%f - @ %f,%f ", lat, lng, global_lat, global_lng); DBG(txt);
			sprintf(txt, "delta %f %f = \0", (float)dlat, (float)dlng); DBG(txt);
			sprintf(txt, "interpolate %f %f %d = \0", (float)int_lat, (float)int_lng,int_step); DBG(txt);
		}
	}


	feet = GPSDistance(lat, lng,global_lat,global_lng);

#ifdef PCMODE
//	sprintf(txt, "%f,%f,%f,%f,%d,%d\r", lat,lng,global_lat, global_lng, int_step,feet);
//	if (traceit==type) GTRACE(txt);

	sprintf(txt, "%f,%f\r", global_lat,global_lng);
	GTRACE(txt);
#endif
	// so lets set the tolerance to those values
	if (feet<bubble)
	{
		match = 1;
	}

	if (traceit & type)
	{
		sprintf(txt, " bubble=%d distance=%d match=%d tps=%d\r\n", bubble, feet,match, global.tps); DBG(txt);
	}
#ifdef GPS_MATCH_TIMING
	BLUE_LEDOff();
#endif

	last_lat = global.gps_lat; // update based on the values from the IRQ
	last_lng = global.gps_lng;

	return match;

}




void HandleSegments(void)
{
        if (minimum_latched)
        {
           Announce(MINIMUM_SPEED);
           minimum_latched=0;
        }
        
        
        if (max_lateral_latched)
        {
            Announce(MAX_LATERAL);
            max_lateral_latched=0;
        }
        
        if (max_linear_latched)
        {
            Announce(MAX_LINEAR);
            max_linear_latched=0;
        }
        if (split_latched)
        {
#ifdef PCMODE
			DBG("--->ANNOUNCE SPLIT<---\0");
#endif
			Announce(SPLIT);
            split_latched=0;
        }
        
        if (entry_latched)
        {
            Announce(ENTRY_SPEED);
            entry_latched=0;
        }
        if (exit_latched)
        {
            Announce(EXIT_SPEED);
            exit_latched=0;
        }
        if (turn_in_latched)
        {
            Announce(TURNIN_SPEED);
            turn_in_latched=0;
        }
}


void LapStatusIRQ(void)
{
#ifdef PCMODE
	char msg[128];
#endif
    
    
    		// increment lap time as the running time is building
            if (global.lap_time>global.last_lap_time)
            {
                global.last_lap_time=global.lap_time;
                global.last_delta_time=global.delta_time;
            }
            // now last lap time is something like 1.32.0
            // then it goes back to 0 when we come across start finish
            if (GPSMatch(settings.checker_lattitude,settings.checker_longitude,CHECKER) && global.new_lap_started>=LAP_HYST) 
            {
				LapTimer(0,1); // capture the current lap_time into global.lap_time and restart the timer
                global.last_lap_time=0;
                if (global.best_lap_time==0 && global.lapnumber>=2)
                {
                    global.best_lap_time=global.lap_time;
                }
                else
                {
                    if (global.lap_time<global.best_lap_time)
                    {
                        global.best_lap_time=global.lap_time;
                        global.new_best_time=1;
                    }
                }

				global.new_lap_started=0;
                global.lapnumber++;  
                global.lap_complete=1; // announce last captured lap time
                global.split_reset=1;

#ifdef PCMODE
				sprintf(msg, "************* NEW LAP Current=%f Last=%f Lap#=%d\r\n", global.lap_time, global.last_lap_time,global.lapnumber);
				DBG(msg);
#endif
            }
			else
			{
				// dont advance the lap hystersysis unless we are actually moving
				if (global.mph>settings.minmph_threshold)
				{
					if (global.new_lap_started<(LAP_HYST<<2))
					{
						global.new_lap_started++;
					}
				}
			}
            

}


void HandleLinearIRQ(void)
{
    static double ling=0;
    static int latching=0;
    double lg=fabs(global.linear_g);
    
#ifdef PCMODE  
 //   if (latching) DBG("LINEAR LATCH ON\r\n"); else DBG("LINEAR LATCH OFF\r\n");
#endif
    if (lg>=settings.linear_g_high_trigger)
    {
        latching=1;
        if (lg>ling) ling=lg; // begin recording
    }
    else
    {
        if (latching)
        {
            global.linear_g_triggered=lg;
        }
        latching=0;
        ling=0;
    }
    
}

void HandleSegmentsIRQ(void)
{
    char mm[64];
    static int i=0;
    static int si=0;
    int j=0;
    int nextseg=0;
    static int state=0;
    static int split_state=0;
    static int mph=1000;
#ifdef PCMODE
	char txt[64];
#endif
#ifdef TRACE_GPS
//    sprintf(mm,"Split=%d Seg=%d,%d  maxlin=%f lateralg=%f\r\n",split_state,state,i,global.latched_max_linear,global.lateral_g); DBG(mm);
#endif
    
    LapStatusIRQ(); // update running lap variables
    
    // make sure we don't get stuck if we get a weak signal
    // and we haven't found the exit of the corner yet
    if (global.split_reset)
    {
		global.split_reset=0;
        memset(split_was_latched,0,sizeof(split_was_latched));
        if (split_state!=0) 
        {
            si=0;
            split_state=0;
        }
        
        if (state!=0)
        {
            i=0;
            state=0;
        }
    }


	//printf("SplitState=%d index=%dr\n",split_state,si);
    switch(split_state)
    {
        case 0:
            if (settings.split_enable[si]==0)
            {
                split_state=2;
            }
            else
            {
               // find the start transition
#ifdef PCMODE
				sprintf(txt, "Looking for split %d start positon\r\n",i); DBG(txt);
#endif
                if (GPSMatch(settings.split_lat[si],settings.split_lng[si],SPLITS))
                {
                  if (split_was_latched[si]==0) split_latched=1;
                  split_state=1;
                }

            }
            break;
            
        case 1:
            // wait until we have moved beyond the split point
#ifdef PCMODE
				sprintf(txt, "Waiting for split %d to end\r\n",i); DBG(txt);
#endif
            if (!GPSMatch(settings.split_lat[si],settings.split_lng[si],SPLITS))     split_state=2;
            break;
            
        case 2:
             // find the next segment in the segment table...
            nextseg=0; 
            for (j=(si+1);j<MAX_SEGMENTS;j++)
            {
				//printf("Check Split=%d,%d\r\n",j,settings.split_enable[j]);
                if (settings.split_enable[j])
                {
                    nextseg=1;
                    si=j;
                    break;
                }
            }
            if (nextseg==0)
            {
                si=0; // back to the first segment
                split_state=3;
            }
            else
            {
                split_state=0;
            }
			break; 

        case 3:
		//	while(1) printf("!!!\r\n");
            split_state=3; // wait here until we come around on the next lap! 
                     // this way if we are under yellow we won't keep triggering the split time bubble
                     // if it is the only bubble that is selected
            break;
            
        default: si=0; split_state=0; break;
           
    }
   
    switch(state)
    {
        // wait for a start transition
        case 0:
            mph=1000;
            global.throttle_timer=0;
            global.rolling_lateral_g=0;
            gps_stage=0;
            if (settings.segment_enable[i]==0)
            {
                state=2; // find next possible segment
            }
            else
            {
                // find the start transition
#ifdef PCMODE
				sprintf(txt, "Looking for segment %d start positon\r\n",i); DBG(txt);
#endif
				if (GPSMatch(settings.segment_start_lat[i],settings.segment_start_lng[i],SEGMENT_START))
                {
                    state=1;
                    global.latched_max_lateral=0;
                    global.latched_entry_speed=0;
                    global.latched_exit_speed=0;
                    global.latched_turn_in_speed=0;
                    global.latched_max_linear=0;
                    entry_latched=0;
                    turn_in_latched=0;
                    exit_latched=0;
                    max_lateral_latched=0;
                    max_linear_latched=0;


#ifdef PCMODE
					sprintf(txt, "Found segment %d start positon\r\n", i); DBG(txt);
#endif

                }
                
                // find any start transition??
                
                
            }
            break;
            
        case 1:
            
#if(0)
            // throttle off-to-on timing
            if (global.throttle_timer!=2)
            {
                    // wait for throttle to go below tps low
                if (global.throttle_timer==0)
                {
                    if (global.tps<settings.tps_low)
                    {
                        global.throttle_timer=1; // now start timing
                
                    }
                }
                else
                {
                    if (global.tps>settings.tps_high)
                    {
                        global.throttle_timer_latched=global.tenths_timer;
                        global.throttle_timer=2;
                    }
                }
            }
#endif
            
            if (settings.segment_enable[i] & EN_ROLLING_LATERAL) global.rolling_lateral_g=1; // allow per corner lateral-g announcement

            if (settings.segment_enable[i] & EN_SEGMENT_MPH) global.segment_mph=1; // allow per corner lateral-g announcement

            // determine entry,turn-in and exit speed
            if (global.latched_entry_speed==0)
            {
                if (settings.segment_enable[i]&EN_ENTRY_SPEED)
                {
                    if (global.tps<settings.tps_low) { entry_latched=1; global.latched_entry_speed=global.mph;}
                }
            }

            
            if (global.latched_turn_in_speed==0)
            {
                    if (global.lateral_g>settings.corner_lateral_g_trigger) 
                    {
                       if (settings.segment_enable[i]&EN_TURN_IN_SPEED) turn_in_latched=1; 
                       if (settings.segment_enable[i]&EN_MAX_LINEAR) max_linear_latched=1;
                       global.latched_turn_in_speed=global.mph;
                    }
                
            }
            
            // latch max lateral g in the turns
            if (global.lateral_g>global.latched_max_lateral)
            {
                global.latched_max_lateral=global.lateral_g;
            }
            if (global.linear_g>global.latched_max_linear)
            {
                global.latched_max_linear=global.linear_g;
            }

            if (global.mph<mph)
            {
                mph=global.mph; // track the minimum
            }
            
            // find the stop transition
            if (gps_stage==0)
            {
#ifdef PCMODE
				sprintf(txt, "Looking for segment %d stop positon\r\n", i); DBG(txt);
#endif
				if (GPSMatch(settings.segment_stop_lat[i], settings.segment_stop_lng[i],SEGMENT_STOP))
				{
#ifdef PCMODE
					sprintf(txt, "Found segment %d stop positon\r\n", i); DBG(txt);
#endif

					gps_stage = 1;
				}
            }
            
            // wait until we leave the stop transition
            if (gps_stage==1)
            {
#ifdef PCMODE
				sprintf(txt, "Looking for segment %d stop to complete is WAITING\r\n", i); DBG(txt);
#endif
				//if (!GPSMatch(settings.segment_stop_lat[i], settings.segment_stop_lng[i]))
				{
					gps_stage = 2;
#ifdef PCMODE
					sprintf(txt, "Looking for segment %d stop to complete is DONE\r\n", i); DBG(txt);
#endif
				}
            }
            
            if (gps_stage==2)
            {
               if (settings.segment_enable[i]&EN_EXIT_SPEED)
               {
                   global.latched_exit_speed=global.mph; 
                    exit_latched=1; 
               }
			   

                if (settings.segment_enable[i]&EN_MINIMUM_SPEED)
                {
                        global.latched_minspeed=mph;
                        minimum_latched=1;
                }
                if (settings.segment_enable[i]&EN_MAX_LATERAL) max_lateral_latched=1;
                
                    state=2;
            }
            
            break;
            
        case 2:
            // find the next segment in the segment table...
            global.segment_mph=0;
            nextseg=0;
            for (j=(i+1);j<MAX_SEGMENTS;j++)
            {
                if (settings.segment_enable[j])
                {
                    nextseg=1;
                    i=j;
                    break;
                }
            }
            if (nextseg==0) i=0; // back to the first segment
            state=0;
            break;
            
        default: state=0; i=0; break;
            
        
    }
    
    
}




void HandleCritical(void)
{
    static unsigned long last_canrx=0;
    static unsigned long last_time=0;
    static int xcycles=0;
    unsigned char gps_wrong=0;
    static unsigned char gps_was_wrong=0;
    char msg[64];

    if (!global.can_sim)
    {
        if (startup_done && global.no_connection==0 && global.rep_trigger==0)
        {
            if (settings.dash_type!=VBOX)
            {
                if (settings.dash_type!=OBD_II)
                {
                    if (global.oil_pressure_psi<=settings.oil_low && settings.oil_announce && global.rpm>=settings.rpm_oil_threshold)
                    {   
                        global.rep_trigger=2;
                        Announce(OIL);
                    }
                }
                if (global.engine_temp_f>=settings.temp_high_f && settings.temp_announce)
                {
                    global.rep_trigger=1;
                    Announce(TEMP);
                }
                // the Aim Solo2DL can ouput a voltage of ~3.7 depending
                // on what canbus signal is mapped. if the car voltage is down to 3.7...it shut down long ago
                if ((int)global.volts>MIN_ANNOUNCE_VOLTS && global.volts<settings.volts && settings.volts_announce) 
                {   
                    global.rep_trigger=1;
                    Announce(VOLTS);
                }
            }
        }
    }
    
    
    //sprintf(msg,"%d %d\r\n",global.sys_timer,last_time); DBG(msg);
    if ((global.sys_timer-last_time)>=5)
    {
        xcycles++; // secondary 5 second interval rate
        if (last_canrx==global.canrx)
        {
                global.no_connection=1;
#ifndef NO_BACKGROUND_ERROR
                if (global.board_version!=RACEVOICE_STANDALONE)  Announce(NO_SIGNAL);
#endif
        }
        else global.no_connection=0;
        
        if (xcycles>1 && (global.board_version==RACEVOICE_STANDALONE || (global.no_connection==0)))
        {
            xcycles=0;
            if (global.gps_lat<((double)-90) || global.gps_lat>((double)90)) gps_wrong=1;
            if (global.gps_lng<((double)-180) || global.gps_lng>((double)180)) gps_wrong=1;
            if (global.gps_lng==(double)0 || global.gps_lat==(double)0) gps_wrong=1;
            
            if (gps_wrong) global.gps_error=1; else global.gps_error=0;
#ifndef NO_BACKGROUND_ERROR
            if (gps_wrong) 
            {
                gps_was_wrong=1;
                Announce(GPS_ERROR);
            }
            else
            {
                if (gps_was_wrong)
                {
                    Announce(GPS_VALID);
                }
                gps_was_wrong=0;
            }
#endif
        }
        last_canrx=global.canrx;
        last_time=global.sys_timer;
    }
}



void HandleBrakeTone(void)
{
    char msg[32];
    static int tone=0;
    
    
#if(1)
#ifdef BETA
#ifdef STEVE
    if (global.brake_front_psi>20 && global.tps>30)
    {
           memset(msg,0,sizeof(msg));
           sprintf(msg,"%dj%04d%04d",settings.brake_tone_duration/5,settings.brake_tone_hz,0);
		   #ifdef PCMODE
		   printf("Brake+Throttle=%d\r\n", settings.brake_tone_hz);

		   #else
		   SpeechByte(1);
           SpeechString(msg,1,1);
		   #endif
    }
#endif
#endif
#endif
    
    if (settings.brake_tone_enable)
    {
        // single shot tone
       if(global.brake_front_psi>=settings.brake_tone_psi_low && global.brake_front_psi<=settings.brake_tone_psi_high && tone==0)
       {
           memset(msg,0,sizeof(msg));
           sprintf(msg,"%dj%04d%04d",settings.brake_tone_duration,settings.brake_tone_hz,0);
#ifdef PCMODE
		   printf(msg);
#else
		   SpeechByte(1);
           SpeechString(msg,1,1);
#endif
           tone=1;
       }
       if (global.brake_front_psi<settings.brake_tone_psi_low) tone=0;
    }
    else
    {
        tone=0;
    }
    
}

void HandlePhrases(void)
{
    static int phrase_counter[MAX_PHRASE+2];
    static char pm[48]; 
    char ch;
    char txt[32];
    static int state=0;
    int j;
    int valid=0;
    unsigned short bitvec;
   
    bitvec=1<<state;
    if (bitvec & global.phrase_triggers) valid=1; // bit bits 7..0
   // sprintf(txt,"state=%d 0x%02x 0x%02x\r\n",state,bitvec,global.phrase_triggers); DBG(txt);
    if (valid)
    {
        if (pm[0]!=0) memset(pm,0,sizeof(pm));

            for (j=0;j<PHRASE_LEN;j++)
            {
                ch=settings.phrase[state][j];
                if (ch==0) break;
                if (ch=='_') ch=' ';
                pm[j]=ch;
            }
            if (phrase_counter[state]<settings.phrase_control[state])
            {
                sprintf(txt,"speak=[%s],%d,%d\r\n",pm,phrase_counter[state],settings.phrase_control[state]); DBG(txt);
                phrase_counter[state]++;
                Speak(pm,3); 
            }
    }
    else
    {
        phrase_counter[state]=0;
    }  
    
    
    state++;
    if (state>=8) state=0;
    
      
    
}

void HandleTires(void)
{
    char msg[32];
    int is_low=0;
    int is_high=0;
    // we probably have some tire pressure data
    if (global.tire_pressure[0]!=0 && global.tire_pressure[1]!=0 && settings.tire_low_psi[0]!=0 && settings.tire_high_psi[0]!=0)
    {
        if (global.tire_pressure[0]>settings.tire_high_psi[0]) is_high|=0x01;
        if (global.tire_pressure[1]>settings.tire_high_psi[0]) is_high|=0x02;
        if (global.tire_pressure[2]>settings.tire_high_psi[0]) is_high|=0x04;
        if (global.tire_pressure[3]>settings.tire_high_psi[0]) is_high|=0x08;

        if (global.tire_pressure[0]<settings.tire_low_psi[0]) is_low|=0x01;
        if (global.tire_pressure[1]<settings.tire_low_psi[0]) is_low|=0x02;
        if (global.tire_pressure[2]<settings.tire_low_psi[0]) is_low|=0x04;
        if (global.tire_pressure[3]<settings.tire_low_psi[0]) is_low|=0x08;
        
        if (is_high==0xf)
        {
            Speak("Tires Are Hot\0",3);
            return;
        }

        if (is_low==0xf)
        {
            Speak("Tires Are Low\0",3);
            return;
        }

        if (is_high==0 && is_low==0)
        {
            Speak("Tires Are Good\0",3);
            return;
        }
        
        if (is_high & 0x01) Speak("Front Left Hot\0",3);
        if (is_low & 0x01)  Speak("Front Left Cold\0",3);

        if (is_high & 0x02) Speak("Front Right Hot\0",3);
        if (is_low & 0x02)  Speak("Front Right Cold\0",3);

        if (is_high & 0x04) Speak("Rear Left Hot\0",3);
        if (is_low & 0x04)  Speak("Rear Left Cold\0",3);

        if (is_high & 0x08) Speak("Rear Right Hot\0",3);
        if (is_low & 0x08)  Speak("Rear Right Cold\0",3);
        

        
    }
    
}
// Process the track feedback code
// This task runs on all current global variable data from the dash
// This tells the driver advanced information such as g-force, split times, and adaptive braking/minimal speeds
// It also has global announce conditions for rpm and faults

void HandleLap(void)
{
    

    
    if (global.throttle_timer==2)
    {
        Announce(TPS_OFF_TO_ON);
        global.throttle_timer=0;
    }
    
    if (global.lap_complete)
    {
        HandleTires();
      
         if (settings.lap_announce & 1)
        {
            if (global.new_best_time)
            {
                Announce(BEST);
                global.new_best_time=0;
            }
            else
            {
                if (settings.lap_announce & 2) Announce(LAP_GAIN);
            }
        }
         else
         {
            if (settings.lap_announce & 2)
            {
             Announce(LAP_GAIN);
            }
         }
        global.lap_complete=0;

    }
    
    
    

}



double ScaleTyre(int type, unsigned short val)
{
    double t=0;
    // kpa to psi
    if (type==0)
    {
        t=(double)val;
        t*=0.145038; // 1kp is 0.145038 psi
        return t;
    }
    
    // celcius to f
    if (type==1)
    {
           t=((float)val * (float)9/(float)5);
           t+=32; // to degree F
           return t;
    }
    
    return 0;
}

/*
 The sinusoidal tone generator is activated with the command
nJaaaabbbb. N specifies the tone duration in 10 ms increments,
between 1 and 59999. A and b specify the frequencies of the two
generators. Note that all eight digits for a and b must be included in
the command. For example, the command
CTRL+A "100j03500440"
generates a 350/440 Hz tone pair (a dial tone) for 1 second.
 */

void ShiftTone(void)
{
    char msg[32];
    int hz=0;
    int loop=0;
    int active=0;
    memset(msg,0,sizeof(msg));
    if (global.tps>settings.tps_high && global.rpm>=settings.rpm_high) active=1;
    while(active)
    {
        for (loop=0;loop<5;loop++)
        {
            hz=settings.shift_tone_hz;
            sprintf(msg,"%dj%04d%04d",settings.shift_tone_duration,hz,0);
#ifdef PCMODE
			sprintf(msg, "Shift Tone=%d\r\n", hz);
			printf(msg);
#else
			SpeechByte(1);
            SpeechString(msg,1,1);
#endif
        }
        if (global.tps<settings.tps_high) active=0;
        if (global.rpm<settings.rpm_high) active=0;
    }
}
