//***********************************************************************************
//** Top Level of the CANBUS<>Voice Application Code
//***********************************************************************************
#ifdef PCMODE
	#include <stdio.h>
	#include <string.h>
	#include <stdlib.h>
	#include <math.h>
	#include <sys\timeb.h>
	#include "defs.h"
	#define uint8_t unsigned char
	#define uint16_t unsigned short
	static char fn[512];
#else 

    #include "defs.h"
	#include "app.h"
	#include <stdio.h>
	#include <string.h> 
	#include <math.h>
	#include <stdlib.h>
    #include "hex.h"
	extern APP_DATA appData;
    extern int FlashWriteAccess(unsigned long, unsigned char*, unsigned long);
    extern int FlashSetup(void);
	static CAN_TX_MSG_BUFFER  can_message_buffer0[1*2*16];
	static uint16_t canTXB[TXB_SIZE];
	extern const unsigned long cantrace_count;
	extern const unsigned char cantrace[];
#endif

static int init_was_done=0;
void InitSettings(void);
uint16_t canRXB[32];
static char delta_string[32];
static char speed_string[32];
void SpeechByte(unsigned char);
int SpeechString(char*,int,int); 
void DBG(char* txt);
double ScaleDouble(unsigned short,long);
extern double MotecScaleDouble(unsigned short,long);
extern double AimScaleDouble(unsigned short,long);
extern double VboxScaleDouble(unsigned short,long);
extern long MotecScale(unsigned short,long);
extern long VboxScale(unsigned short,long);
extern long AimScale(unsigned short,long);
extern void B_UTX(char);
extern int PageErase(unsigned long);
extern int WordPGM(unsigned long,unsigned long);
extern int EraseFlashSegment(int);
extern void FlashStatus(void);
void SetBaud(int);
void FlashCanLED(void);
void HandleLap(void);
void SetVoiceSpeed(int);
void ProcessTrackFeedback(void);
void SendStop(void);
void ShiftTone(void);
static int fw_update=0;
static char txttmp[128];
static int direct_rc_link=0;
int GetChSp(void);
int GetCh(void);
void LapTimer(int,int);
static unsigned char new_level_board=0;
extern void PushSpeechBuffer(char);
void HandleCustomSpeech(void);
void HandleCustomSpeechIRQ(unsigned char,unsigned char);
extern void CalcPredictive(void);
extern unsigned long gps_distance(double,double);
extern void GenerateOBDII(void);
extern void WrLogData(void);
extern int HandleLateral(void);
extern void HandleRPM(void);
extern void HandleMPH(int);
extern void HandleLinear(void);
extern void HandleWheelSpeed(void);
extern void HandleBrakeTone(void);
extern void HandlePhrases(void);
extern void HandleSegments(void);
extern void ProcessOBDII(void);
extern void ProcessCustomCan(void);
extern void HandleSegmentsIRQ(void);
extern void HandleLinearIRQ(void);
extern void HandleWheelSpeedIRQ(void);
extern int UpdateRunTimeFlash(void);
extern void GPS_Init(void);
extern void SetupCustomCan(void);
extern void SetupOBD_IICan(void);
extern void HandleStartup(void);
extern void HandleCritical(void);
extern void HandleLocalAccel(void);
extern void HandleBluetooth(void);


struct _settings_ settings;
struct _global_ global;

#define TPS_TICKER 10

char otxt[512];
void ProcessCanRX(void);
int WaitSpeech(unsigned char);
void PlayAudio(int,int);
void PlayAudioInteger(int,int);
extern void HandleSPIFlash(void);
extern int ValidSpiFlash(void);
#ifdef PCMODE
FILE* ofile;
FILE *fw_file;
FILE* gfile;
static unsigned long cantrace_count = 0;
void SetFileName(char* file)
{
	strcpy(fn, file);
}
void CloseFiles(void)
{
	int i;
	if (fw_file != NULL)
	{
		for (i = 0; i < 8; i++) fprintf(fw_file, "0xff,");
		fprintf(fw_file, "0xff};\r\n");
		fprintf(fw_file, "const unsigned long cantrace_count=%ld;\r\n", cantrace_count);
		fclose(fw_file);
		fclose(ofile);
		fclose(gfile);
	}
}
void GenFwArray(void)
{
	static int first = 1;
	int i;
	if (first)
	{
		fw_file = fopen("c:\\microchip\\harmony\\v2_03b\\apps\\VoiceBoard\\firmware\\src\\track.c", "w");
		first = 0;
		if (fw_file!=NULL)
		{
			fprintf(fw_file, "//Firwmare CANDemo File=[%s]\r\n", fn);
			fprintf(fw_file, "const unsigned char cantrace[] = { \r");
		}
	}

	if (fw_file == NULL) return;

	for (i = 0; i < 9; i++)
	{
		fprintf(fw_file, "0x%02x,",canRXB[i]);
	}
	fprintf(fw_file, "\r");
	cantrace_count++;
}
void DBG(char* txt)
{
	sprintf(otxt,"%s",txt);
	printf("%s",otxt);
	fprintf(ofile,"%s",otxt);
}

void GTRACE(char* txt)
{
	static int first = 1;
	if (first)
	{
		gfile= fopen("c:\\temp\\gpstrace.csv", "w");
		first = 0;
	}
	fprintf(gfile, "%s\0", txt);
}
void SetVoiceSpeed(int v)
{
}

void PushCanMessage(unsigned char* msg)
{
	int i;
	static int count = 0;
	static int first=1;
	static int sample = 0;
	if (first)
	{
		InitSettings();


		ofile=fopen("c:\\temp\\voicetrace.txt","w");
		first=0;


#if(0)
		settings.segment_start_lat[0] = 42.3429163;
settings.segment_start_lng[0] = -76.9287773;
settings.segment_stop_lat[0] = 42.34431;
settings.segment_stop_lng[0] = -76.927535;
settings.segment_enable[0] = 1;
settings.segment_start_lat[1] = 42.3347516;
settings.segment_start_lng[1] = -76.9204666;
settings.segment_stop_lat[1] = 42.3312416;
settings.segment_stop_lng[1] = -76.9203366;
settings.segment_enable[1] = 1;
settings.segment_start_lat[2] = 42.3310016;
settings.segment_start_lng[2] = -76.920415;
settings.segment_stop_lat[2] = 42.3313333;
settings.segment_stop_lng[2] = -76.9236816;
settings.segment_enable[2] = 1;
settings.segment_start_lat[3] = 42.33182;
settings.segment_start_lng[3] = -76.92417;
settings.segment_stop_lat[3] = 42.331926;
settings.segment_stop_lng[3] = -76.926462;
settings.segment_enable[3] = 0;
settings.segment_start_lat[4] = 42.3298146;
settings.segment_start_lng[4] = -76.9262409;
settings.segment_stop_lat[4] = 42.3291133;
settings.segment_stop_lng[4] = -76.9278766;
settings.segment_enable[4] = 0;
settings.segment_start_lat[5] = 42.3325116;
settings.segment_start_lng[5] = -76.9282616;
settings.segment_stop_lat[5] = 42.3344136;
settings.segment_stop_lng[5] = -76.927756;
settings.segment_enable[5] = 0;
settings.segment_start_lat[6] = 42.333657;
settings.segment_start_lng[6] = -76.9256279;
settings.segment_stop_lat[6] = 42.334685;
settings.segment_stop_lng[6] = -76.9242383;
settings.segment_enable[6] = 0;
settings.segment_start_lat[7] = 42.3359683;
settings.segment_start_lng[7] = -76.9246233;
settings.segment_stop_lat[7] = 42.3371066;
settings.segment_stop_lng[7] = -76.9255216;
settings.segment_enable[7] = 1;
settings.segment_start_lat[8] = 42.3372369;
settings.segment_start_lng[8] = -76.9276957;
settings.segment_stop_lat[8] = 42.3398333;
settings.segment_stop_lng[8] = -76.929043;
settings.segment_enable[8] = 4;
settings.segment_start_lat[9] = 0;
settings.segment_start_lng[9] = 0;
settings.segment_stop_lat[9] = 0;
settings.segment_stop_lng[9] = 0;


settings.split_lat[0] = 42.340975;
settings.split_lng[0] = -76.9227316;
settings.split_enable[0] = 0;
settings.split_lat[1] = 42.336039;
settings.split_lng[1] = -76.9205326;
settings.split_enable[1] = 1;
settings.split_lat[2] = 42.330615;
settings.split_lng[2] = -76.92294;
settings.split_enable[2] = 0;
settings.split_lat[3] = 42.3304433;
settings.split_lng[3] = -76.92631;
settings.split_enable[3] = 0;
settings.split_lat[4] = 42.3315263;
settings.split_lng[4] = -76.9281396;
settings.split_enable[4] = 0;
settings.split_lat[5] = 42.3338713;
settings.split_lng[5] = -76.9261336;
settings.split_enable[5] = 0;
settings.split_lat[6] = 42.337262;
settings.split_lng[6] = -76.9262626;
settings.split_enable[6] = 0;
settings.split_lat[7] = 42.3394016;
settings.split_lng[7] = -76.9290766;
settings.split_enable[7] = 0;
settings.split_lat[8] = 0;
settings.split_lng[8] = 0;
settings.split_enable[8] = 0;
settings.split_lat[9] = 0;
settings.split_lng[9] = 0;
settings.split_enable[9] = 0;
settings.checker_lattitude=42.3409966;
settings.checker_longitude=-76.9289516;
#endif



#if(0)
// thompson
settings.segment_start_lat[0] = 41.9802626;
settings.segment_start_lng[0] = -71.8284923;
settings.segment_stop_lat[0] = 41.9816649;
settings.segment_stop_lng[0] = -71.8298303;
settings.segment_enable[0] = 0;
settings.segment_start_lat[1] = 41.9811956;
settings.segment_start_lng[1] = -71.829001;
settings.segment_stop_lat[1] = 41.9813083;
settings.segment_stop_lng[1] = -71.8281066;
settings.segment_enable[1] = 1;
settings.segment_start_lat[2] = 41.9816666;
settings.segment_start_lng[2] = -71.827785;
settings.segment_stop_lat[2] = 41.9816316;
settings.segment_stop_lng[2] = -71.825455;
settings.segment_enable[2] = 1;
settings.segment_start_lat[3] = 41.9826073;
settings.segment_start_lng[3] = -71.8253043;
settings.segment_stop_lat[3] = 41.982165;
settings.segment_stop_lng[3] = -71.823885;
settings.segment_enable[3] = 64;
settings.segment_start_lat[4] = 41.9807333;
settings.segment_start_lng[4] = -71.8238416;
settings.segment_stop_lat[4] = 41.9795057;
settings.segment_stop_lng[4] = -71.8220059;
settings.segment_enable[4] = 1;
settings.segment_start_lat[5] = 41.9787916;
settings.segment_start_lng[5] = -71.8222633;
settings.segment_stop_lat[5] = 41.978879;
settings.segment_stop_lng[5] = -71.825271;
settings.segment_enable[5] = 4;
settings.segment_start_lat[6] = 0;
settings.segment_start_lng[6] = 0;
settings.segment_stop_lat[6] = 0;
settings.segment_stop_lng[6] = 0;
settings.segment_enable[6] = 0;
settings.segment_start_lat[7] = 0;
settings.segment_start_lng[7] = 0;
settings.segment_stop_lat[7] = 0;
settings.segment_stop_lng[7] = 0;
settings.segment_enable[7] = 0;
settings.segment_start_lat[8] = 0;
settings.segment_start_lng[8] = 0;
settings.segment_stop_lat[8] = 0;
settings.segment_stop_lng[8] = 0;
settings.segment_enable[8] = 0;
settings.segment_start_lat[9] = 0;
settings.segment_start_lng[9] = 0;
settings.segment_stop_lat[9] = 0;
settings.segment_stop_lng[9] = 0;
settings.segment_enable[9] = 0;
settings.split_lat[0] = 41.9804;
settings.split_lng[0] = -71.8289033;
settings.split_enable[0] = 1;
settings.split_lat[1] = 41.9830436;
settings.split_lng[1] = -71.8242723;
settings.split_enable[1] = 1;
settings.split_lat[2] = 41.97844;
settings.split_lng[2] = -71.8229966;
settings.split_enable[2] = 1;
settings.split_lat[3] = 0;
settings.split_lng[3] = 0;
settings.split_enable[3] = 0;
settings.split_lat[4] = 0;
settings.split_lng[4] = 0;
settings.split_enable[4] = 0;
settings.split_lat[5] = 0;
settings.split_lng[5] = 0;
settings.split_enable[5] = 0;
settings.split_lat[6] = 0;
settings.split_lng[6] = 0;
settings.split_enable[6] = 0;
settings.split_lat[7] = 0;
settings.split_lng[7] = 0;
settings.split_enable[7] = 0;
settings.split_lat[8] = 0;
settings.split_lng[8] = 0;
settings.split_enable[8] = 0;
settings.split_lat[9] = 0;
settings.split_lng[9] = 0;
settings.split_enable[9] = 0;
settings.checker_lattitude = 41.9796553;
settings.checker_longitude = -71.8270116;
#endif

#if(0)
// indy
settings.segment_start_lat[0] = 39.7981639092701;
settings.segment_start_lng[0] = -86.2389938513043;
settings.segment_stop_lat[0] = 39.7991705;
settings.segment_stop_lng[0] = -86.2386265;
settings.segment_enable[0] = 1;
settings.segment_start_lat[1] = 39.7991871;
settings.segment_start_lng[1] = -86.2383743;
settings.segment_stop_lat[1] = 39.7998264930903;
settings.segment_stop_lng[1] = -86.2376193013218;
settings.segment_enable[1] = 0;
settings.segment_start_lat[2] = 39.8010799;
settings.segment_start_lng[2] = -86.2367783;
settings.segment_stop_lat[2] = 39.8006403;
settings.segment_stop_lng[2] = -86.2354837;
settings.segment_enable[2] = 0;
settings.segment_start_lat[3] = 39.8000161599122;
settings.segment_start_lng[3] = -86.2354338346829;
settings.segment_stop_lat[3] = 39.7985236597494;
settings.segment_stop_lng[3] = -86.2349304346894;
settings.segment_enable[3] = 0;
settings.segment_start_lat[4] = 39.7931111762303;
settings.segment_start_lng[4] = -86.2349788013554;
settings.segment_stop_lat[4] = 39.792212;
settings.segment_stop_lng[4] = -86.2343247;
settings.segment_enable[4] = 0;
settings.segment_start_lat[5] = 39.7922504;
settings.segment_start_lng[5] = -86.2336877;
settings.segment_stop_lat[5] = 39.7915831430517;
settings.segment_stop_lng[5] = -86.2319960513934;
settings.segment_enable[5] = 0;
settings.segment_start_lat[6] = 39.7915666;
settings.segment_start_lng[6] = -86.2315168;
settings.segment_stop_lat[6] = 39.7904000266011;
settings.segment_stop_lng[6] = -86.2309664680732;
settings.segment_enable[6] = 0;
settings.segment_start_lat[7] = 39.7881373930982;
settings.segment_start_lng[7] = -86.2337034680383;
settings.segment_stop_lat[7] = 39.7884947;
settings.segment_stop_lng[7] = -86.2353471;
settings.segment_enable[7] = 0;
settings.segment_start_lat[8] = 39.789058;
settings.segment_start_lng[8] = -86.2352364;
settings.segment_stop_lat[8] = 39.7893865;
settings.segment_stop_lng[8] = -86.2364941;
settings.segment_enable[8] = 0;
settings.segment_start_lat[9] = 0;
settings.segment_start_lng[9] = 0;
settings.segment_stop_lat[9] = 0;
settings.segment_stop_lng[9] = 0;
settings.segment_enable[9] = 0;
settings.split_lat[0] = 39.7975102391972;
settings.split_lng[0] = -86.2389775763045;
settings.split_enable[0] = 0;
settings.split_lat[1] = 39.8006529929778;
settings.split_lng[1] = -86.2373352513254;
settings.split_enable[1] = 0;
settings.split_lat[2] = 39.7979779828592;
settings.split_lng[2] = -86.2349362552021;
settings.split_enable[2] = 0;
settings.split_lat[3] = 39.7956998309604;
settings.split_lng[3] = -86.2349993895904;
settings.split_enable[3] = 0;
settings.split_lat[4] = 39.793751087599;
settings.split_lng[4] = -86.2349907013552;
settings.split_enable[4] = 0;
settings.split_lat[5] = 39.7897439265944;
settings.split_lng[5] = -86.2311330014044;
settings.split_enable[5] = 0;
settings.split_lat[6] = 39.7882362;
settings.split_lng[6] = -86.2330318;
settings.split_enable[6] = 0;
settings.split_lat[7] = 39.7893423;
settings.split_lng[7] = -86.2375829;
settings.split_enable[7] = 0;
settings.split_lat[8] = 0;
settings.split_lng[8] = 0;
settings.split_enable[8] = 0;
settings.split_lat[9] = 0;
settings.split_lng[9] = 0;
settings.split_enable[9] = 0;
settings.checker_lattitude=39.7931512;
settings.checker_longitude=-86.2389124;
#endif


#if(1)
//nhms
settings.segment_start_lat[0] = 43.36196957;
settings.segment_start_lng[0] = -71.46262423;
settings.segment_stop_lat[0] = 43.36017347;
settings.segment_stop_lng[0] = -71.46015537;
settings.segment_enable[0] = 129;
settings.segment_start_lat[1] = 43.36155716;
settings.segment_start_lng[1] = -71.45936643;
settings.segment_stop_lat[1] = 43.36177828;
settings.segment_stop_lng[1] = -71.45840261;
settings.segment_enable[1] = 1;
settings.segment_start_lat[2] = 43.36108959;
settings.segment_start_lng[2] = -71.45891527;
settings.segment_stop_lat[2] = 43.36009241;
settings.segment_stop_lng[2] = -71.45836097;
settings.segment_enable[2] = 64;
settings.segment_start_lat[3] = 43.36173681;
settings.segment_start_lng[3] = -71.45798104;
settings.segment_stop_lat[3] = 43.36388483;
settings.segment_stop_lng[3] = -71.45872759;
settings.segment_enable[3] = 0;
settings.segment_start_lat[4] = 43.36473271;
settings.segment_start_lng[4] = -71.45869628;
settings.segment_stop_lat[4] = 43.3647868;
settings.segment_stop_lng[4] = -71.46102073;
settings.segment_enable[4] = 6;
settings.segment_start_lat[5] = 0;
settings.segment_start_lng[5] = 0;
settings.segment_stop_lat[5] = 0;
settings.segment_stop_lng[5] = 0;
settings.segment_enable[5] = 0;
settings.segment_start_lat[6] = 0;
settings.segment_start_lng[6] = 0;
settings.segment_stop_lat[6] = 0;
settings.segment_stop_lng[6] = 0;
settings.segment_enable[6] = 0;
settings.segment_start_lat[7] = 0;
settings.segment_start_lng[7] = 0;
settings.segment_stop_lat[7] = 0;
settings.segment_stop_lng[7] = 0;
settings.segment_enable[7] = 0;
settings.segment_start_lat[8] = 0;
settings.segment_start_lng[8] = 0;
settings.segment_stop_lat[8] = 0;
settings.segment_stop_lng[8] = 0;
settings.segment_enable[8] = 0;
settings.segment_start_lat[9] = 0;
settings.segment_start_lng[9] = 0;
settings.segment_stop_lat[9] = 0;
settings.segment_stop_lng[9] = 0;
settings.segment_enable[9] = 0;
settings.split_lat[0] = 43.35992761;
settings.split_lng[0] = -71.46213059;
settings.split_enable[0] = 0;
settings.split_lat[1] = 43.36144913;
settings.split_lng[1] = -71.45938992;
settings.split_enable[1] = 0;
settings.split_lat[2] = 43.36145899;
settings.split_lng[2] = -71.45861942;
settings.split_enable[2] = 0;
settings.split_lat[3] = 43.3608085;
settings.split_lng[3] = -71.45837717;
settings.split_enable[3] = 0;
settings.split_lat[4] = 43.36405667;
settings.split_lng[4] = -71.45866736;
settings.split_enable[4] = 0;
settings.split_lat[5] = 43.36387716;
settings.split_lng[5] = -71.46172165;
settings.split_enable[5] = 0;
settings.split_lat[6] = 0;
settings.split_lng[6] = 0;
settings.split_enable[6] = 0;
settings.split_lat[7] = 0;
settings.split_lng[7] = 0;
settings.split_enable[7] = 0;
settings.split_lat[8] = 0;
settings.split_lng[8] = 0;
settings.split_enable[8] = 0;
settings.split_lat[9] = 0;
settings.split_lng[9] = 0;
settings.split_enable[9] = 0;
settings.checker_lattitude = 43.36315625;
settings.checker_longitude = -71.46206067;



#endif
	//	settings.checker_lattitude = 42.34089755;
	//	settings.checker_longitude = -76.92890916;

		//settings.lateral_gforce_announce = 1;

		
	}


	for (i = 0;i < 9;i++) canRXB[i] = *(msg + i);

	if (canRXB[0] == 0xDE)
	{
		global.read_from_flash = 2;
		while (global.read_from_flash != 0)
		{
			HandleSPIFlash();
		}
		return;
	}
	GenFwArray();
	ProcessCanRX();
	ProcessTrackFeedback();
	if (sample >= 10)
	{
		WrLogData();
		sample = 0;
	} 

	sample++;
for (i=0;i<50;i++)	HandleSPIFlash();



}
#endif

#ifndef PCMODE

void Reboot(void)
{
    SYSKEY = 0x0; // ensure OSCCON is locked
    SYSKEY = 0xAA996655; // Write Key1 to SYSKEY
    SYSKEY = 0x556699AA; // Write Key2 to SYSKEY
    // OSCCON is now unlocked
    RSWRST=1;
    while(RSWRST)
    {
        asm("nop");
    }
    while(1) asm("nop");
}

#endif


void msleep(unsigned long ms)
{
#ifndef PCMODE
	unsigned long i,j;
    for (i=0;i<ms;i++)
    {
        for (j=0;j<10000;j++)
        {
            asm("nop");
        }
    }
#endif
}


void SpeakControl(int mode, unsigned char val)
{
#ifndef PCMODE
	// set volume
    char tv[10];
    char msg[32];
    memset(tv,0,sizeof(tv));
    sprintf(tv,"%d\0",val);
   // sprintf(msg,"speak=0x%02x\r\n",val); DBG(msg);
    switch(mode)
    {
        case VOLUME:
            SpeechByte(1);
            SpeechString(tv,0,1);
            SpeechByte('V');
            break;

        case PITCH:
            SpeechByte(1);
            SpeechString(tv,0,1);
            SpeechByte('P');
            break;
        case REVERB:
            SpeechByte(1);
            SpeechString(tv,0,1);
            SpeechByte('R');
            break;

        case VOICE:
            SpeechByte(1);
            SpeechString(tv,0,1);
            SpeechByte('O');
            break;
        case FREQ:
            SpeechByte(1);
            SpeechString(tv,0,1);
            SpeechByte('F');
            break;
        case TONE:
            SpeechByte(1);
            SpeechString(tv,0,1);
            SpeechByte('X');
            break;

         case SETPOR:
            SpeechByte(1);
            SpeechString(tv,0,1);
            SpeechByte('G');
            break;

        case VOICESPEED:
            SpeechByte(1);
            SpeechString(tv,0,1);
            SpeechByte('S');
            break;
        case TEXTMODE:
            SpeechByte(1);
            SpeechByte('T');
            SpeechByte('/');
            SpeechByte('0');
            SpeechByte('T');
            break;
            
        default: break;
    }
#endif    
}

void InjectSpeech(char* txt)
{
	int i;
	PushSpeechBuffer(0x01); // clear
	for (i = 0;i < strlen(txt);i++)
	{
		PushSpeechBuffer(*(txt + i));
	}
	PushSpeechBuffer(0xaa);
}

int Speak(char* txt,int priority)
{
#ifdef PCMODE
	int i;
	sprintf(otxt,">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>SPEAK:%s",txt);
	printf("%s\r\n",otxt);
	fprintf(ofile,"%s\r\n",otxt);


	PushSpeechBuffer(0); // clear
	for (i = 0;i < strlen(txt);i++)
	{
		PushSpeechBuffer(*(txt + i));
	}
	PushSpeechBuffer(0xaa);

#else
    
    return SpeechString(txt,1,priority);
#endif
}

void ProcessMPH(void)
{
    memset(speed_string,0,sizeof(speed_string));
    if (global.mph<=100)
    {
        sprintf(speed_string,"%d\0",global.mph);
    }
    if (global.mph==101) sprintf(speed_string,"1 O 1\0");
    if (global.mph==102) sprintf(speed_string,"1 O 2\0");
    if (global.mph==103) sprintf(speed_string,"1 O 3\0");
    if (global.mph==104) sprintf(speed_string,"1 O 4\0");
    if (global.mph==105) sprintf(speed_string,"1 O 5\0");
    if (global.mph==106) sprintf(speed_string,"1 O 6\0");
    if (global.mph==107) sprintf(speed_string,"1 O 7\0");
    if (global.mph==108) sprintf(speed_string,"1 O 8\0");
    if (global.mph==109) sprintf(speed_string,"1 O 9\0");
    if (global.mph>=110)
    {
         sprintf(speed_string,"1 %d\0",global.mph-100);   
    }
}

void ProcessDelta(int mode)
{
    int ds=0;
    int idt=0;
    int no_string=0;
    char ts[32];
	double dt=fabs(global.delta_time);
	memset(delta_string,0,sizeof(delta_string));
    memset(ts,0,sizeof(ts));

    // only process delta splits for lap times within 2 seconds of each other
    //if (dt>(double)2)
    //{
     //   return;
    //}
    /*
    if (dt<(double)1)
    {
        idt=(int)(dt * (double)10);
        
        if (idt>0 && idt<=1) sprintf(ts,"ONE TENTH\0");
        if (idt>1 && idt<=2) sprintf(ts,"TWO TENTHS\0");
        if (idt>2 && idt<=3) sprintf(ts,"THREE TENTHS\0");
        if (idt>3 && idt<=4) sprintf(ts,"FOUR TENTHS\0");
        if (idt>4 && idt<=5) sprintf(ts,"FIVE TENTHS\0");
        if (idt>5 && idt<=6) sprintf(ts,"SIX TENTHS\0");
        if (idt>6 && idt<=7) sprintf(ts,"SEVEN TENTHS\0");
        if (idt>7 && idt<=8) sprintf(ts,"EIGHT TENTHS\0");
        if (idt>8 && idt<=9) sprintf(ts,"NINE TENTHS\0");
        if (idt>9 && idt<=10) sprintf(ts,"TEN TENTHS\0");
        
        if (strlen(ts)==0) no_string=1;
        
        if (!no_string)
        {
            if (global.delta_time<(double)0)
         	{
                sprintf(delta_string,"MINUS %s\0",ts);
                ds=1;
            }

            if (global.delta_time>(double)0)
            {
                ds=1;
                sprintf(delta_string,"PLUS %s\0",ts);
            }   
        }
    }

     */
    
         if (global.delta_time<(double)0)
    	{
            sprintf(delta_string,"MINUS %.2f\0",dt);
            ds=1;
        }

        if (global.delta_time>(double)0)
        {
            ds=1;
            sprintf(delta_string,"PLUS %.2f\0",dt);
        }
    

	if (!ds)
 	{
		sprintf(delta_string,"ZERO GAIN\0");
    }
    

}


void IntToText(char value,char* ss)
{
		if (value=='0') sprintf(ss,"OH \0");
		if (value=='1') sprintf(ss,"1 \0");
		if (value=='2') sprintf(ss,"2 \0");
		if (value=='3') sprintf(ss,"3 \0");
		if (value=='4') sprintf(ss,"4 \0");
		if (value=='5') sprintf(ss,"5 \0");
		if (value=='6') sprintf(ss,"6 \0");
		if (value=='7') sprintf(ss,"7 \0");
		if (value=='8') sprintf(ss,"8 \0");
		if (value=='9') sprintf(ss,"9 \0");
}

void AddTime(int value,int type)
{
	char digits[10];
	char ss[32];
	int i;
	sprintf(digits, "%d", value);

	if (type==3)
	{
		if (value<10)
		{
			sprintf(ss,"POINT OH OH %d\0",value);
			strcat(global.timestring,ss);
			return;
		}
		if (value>=10 && value<=99)
		{
			sprintf(ss,"POINT OH %d\0",value);
			strcat(global.timestring,ss);
			return;
		}
		if (value==100 || value==200 || value==300 || value==400 || value==500 || value==600 || value==700 || value==800 || value==900)
		{
			sprintf(ss,"POINT %d \0",value);
			strcat(global.timestring,ss);
			return;
		}

		// value>=100 and not a multiple of 100
		sprintf(ss,"POINT \0");
		strcat(global.timestring,ss);
		IntToText(digits[0],ss);
		strcat(global.timestring,ss); // get the hundreths places
		// extract the tenths and ones to form a new number
		digits[0]=digits[1];
		digits[1]=digits[2];
		digits[2]=0;
		value=atoi(digits);
		if (value>=1 && value<=9)
		{
			sprintf(ss,"OH %d",value);
			strcat(global.timestring,ss);
		}
		else
		{
			sprintf(ss,"%d \0",value);
			strcat(global.timestring,ss);
		}
		return;

	}

	// second
	if (type==2)
	{
		if (value>=10)
		{
			sprintf(ss,"%d \0",value);
			strcat(global.timestring,ss);
		}
		else
		{
			sprintf(ss,"OH %d \0",value);
			strcat(global.timestring,ss);
		}
		return;
	}

	// minute
	if (type==1)
	{
		sprintf(ss,"%d \0",value);
		strcat(global.timestring,ss);
	}
	
}
void TimeConvert(double tv)
{
    long minute = 0;
    long second = 0;
    long milisecond = 0;

    minute = (long)(tv / 60);
    second = (long)(floor(tv) - (minute * 60));
    milisecond = (long)((tv-floor(tv))*1000);

    memset(global.timestring,0,sizeof(global.timestring));
    if (minute!=0)
    {
	AddTime(minute,1);
	}
    AddTime(second,2);
	AddTime(milisecond,3);
    //sprintf(txttmp,"Minute=%d Second=%d Ms=%d\r\n",minute,second,milisecond); DBG(txttmp);
    //sprintf(txttmp,"TimeString = [%s]\r\n",global.timestring); DBG(txttmp);
}

int RateLimiter(void)
{
    static long lt=0;
    int limit=0;
    long dt;

        // limit how fast we speak in back to back messages
        dt=global.sys_timer-lt;
        if (dt<2)
        {
            limit=1;
        }
    
        // save the last time we announced something
        if (limit==0)
        {
            lt=global.sys_timer; 
        }
  return limit;
}




// announce conditions to the driver
void Announce(int mode)
{
    static int remap=0;
    static char msg[128];
    unsigned long replevelo=100;
    unsigned long replevelt=300;
    int trace=1;
    
    trace=1;
    memset(msg,0,sizeof(msg));
    
#ifdef PCMODE
	remap = 0;
#else
	remap = 0;
#endif
    switch(mode)
    {
        case TIRELOCK_FRONT:
            sprintf(msg,"FRONT\0");
            Speak(msg,3);
            break;
        case GPS_ERROR:
            sprintf(msg,"GEE PEA ESS SEARCHING\0");
            Speak(msg,3);
            break;
        case GPS_VALID:
            sprintf(msg,"GEE PEA ESS VALID\0");
            Speak(msg,3);
            break;
        case TIRELOCK_REAR:
            sprintf(msg,"REAR\0");
            Speak(msg,3);
            break;
        case TIRELOCK:
            sprintf(msg,"LOCK UP\0");
            Speak(msg,3);
            break;
        case OVERREV:
            sprintf(msg,"OVER REV\0");
            Speak(msg,3);
            break;
        case MINIMUM_SPEED:
            if (remap)
            {
                PlayAudio(35,trace);
                PlayAudioInteger(global.latched_minspeed,trace);
                break;
            }
            sprintf(msg,"MINIMUM %d\0",global.latched_minspeed);
            Speak(msg,3);
            break;
        case ENTRY_SPEED:
            if (remap)
            {
                PlayAudio(37,trace);
                PlayAudioInteger(global.latched_entry_speed,trace);
                break;
            }
            sprintf(msg,"ENTRY %d\0",global.latched_entry_speed);
            Speak(msg,3);
            break;
        case TURNIN_SPEED:
            if (remap)
            {
                PlayAudio(36,trace);
                PlayAudioInteger(global.latched_turn_in_speed,trace);
                break;
            }
            sprintf(msg,"TURN IN %d\0",global.latched_turn_in_speed);
            Speak(msg,3);
            break;
        case EXIT_SPEED:
            if (remap)
            {
                PlayAudio(38,trace);
                PlayAudioInteger(global.latched_exit_speed,trace);
                break;
            }
            sprintf(msg,"EXIT %d\0",global.latched_exit_speed);
            Speak(msg,3);
            break;
        case TPS_OFF_TO_ON:
            sprintf(msg,"THROTTLE %1.1f\0",global.throttle_timer_latched);
            Speak(msg,3);
            break;
   		case NEW_LAP_LAPTIME:
            TimeConvert(global.lap_time);
            sprintf(msg,"Lap %s\0",global.timestring);
            Speak(msg,3);
            break;
        case MAX_LATERAL:
            sprintf(msg,"%1.1f Lateral\0",global.latched_max_lateral);
            Speak(msg,3);
            break;
        case MAX_LINEAR:
            sprintf(msg,"%1.1f Linear\0",global.latched_max_linear);
            Speak(msg,3);
            break;
        case NEW_LAP:
            sprintf(msg,"NEW LAP\0");
            Speak(msg,3);
            break;
        case BEST:
            sprintf(msg,"BEST LAP\0");
            Speak(msg,3);
            break;
         // debug trace only
        case CURRENT_RPM:
			sprintf(msg,"RPM %d\0",global.rpm);
            Speak(msg,0);
			break;

        case TPS:
			sprintf(msg,"%d\0",global.tps);
            Speak(msg,0);
            break;
            
        case TPS_NOW:
			sprintf(msg,"%d\0",global.tps);
            Speak(msg,1);
            break;
            
        case UP:
            if (remap) 
            {
                PlayAudio(4,trace);
                break;
            }
			sprintf(msg,"UPSHIFT\0");
			Speak(msg,3);
			break;
        case DOWN: 
            if (remap) 
            {
                PlayAudio(5,trace);
                break;
            }
			sprintf(msg,"DOWNSHIFT\0",global.rpm);
			Speak(msg,3);
			break;
	
         case NO_SIGNAL:
            sprintf(msg,"NO CONNECTION\0");
            SetVoiceSpeed(1);
            Speak(msg,3);
            SetVoiceSpeed(0);

            break;
            
        case VOLTS:
            sprintf(msg,"VOLTAGE %.1f\0",global.volts);
                    SetVoiceSpeed(1);
                    Speak(msg,3);
                    SetVoiceSpeed(0);
            break;
            
        case OIL: 
			sprintf(msg,"OIL %d P S I\0",global.oil_pressure_psi);
            Speak(msg,3);
            SetVoiceSpeed(0);
			break;
        case TEMP: 
			sprintf(msg,"TEMPERATURE %d DEGREES\0",global.engine_temp_f);
            SetVoiceSpeed(1);
            Speak(msg,3);
            SetVoiceSpeed(0);
			break;
        
            
		case COAST: 
            ProcessMPH();
            sprintf(msg,"COASTING %s\0",speed_string);
            Speak(msg,0);
            break;
  
        case SPEED: 
                 ProcessMPH();
                 Speak(speed_string,2);
            break;

        case LAP_GAIN:
               	   ProcessDelta(2);
                   sprintf(msg,"%s\0",delta_string);
                   Speak(msg,3);
			break;
        case SPLIT:
               	   ProcessDelta(1);
                   sprintf(msg,"%s\0",delta_string);
                   Speak(msg,3);
			break;
            
		case SPEED_SPLIT:
			ProcessDelta(0);
            ProcessMPH();
            if (strlen(delta_string)>0)
            sprintf(msg,"%s AT %s\0",speed_string,delta_string);
            else
            sprintf(msg,"%s\0",speed_string);
            Speak(msg,3);
			break;

        case LATERAL: 
#ifdef PCMODE
			sprintf(msg,"LATERAL %1.1f\0",global.lateral_g);
#else
			sprintf(msg,"%1.1f\0",global.lateral_g);
#endif
#ifdef DEMO_LOOP
            Speak(msg,1);
#else
            Speak(msg,2);
#endif
            break;
            
        case LINEAR: 

#ifdef PCMODE
            sprintf(msg,"LinearMax %1.1f\0",global.linear_g_triggered);
#else
            sprintf(msg,"Max %1.1f\0",global.linear_g_triggered);
#endif
			Speak(msg,1); 
            break;

        case LATERAL_GO: 
            sprintf(msg,"GO %1.1f \0",global.lateral_g);
            Speak(msg,2);
            break;

		defaut:break;
            
    }
    
}



#ifndef PCMODE
void DumpRawSettings(void)
{
    unsigned char* ch;
    char t[32];
    int i;
    int j=0;
    ch=(unsigned char*)&settings;
    DBG("\r\n");
    for (i=0;i<sizeof(settings);i++)
    {
        sprintf(t,"%02x \0",*ch++);
        DBG(t);
        j++;
        if (j>=16) 
        {
            DBG("\r\n");
            j=0;
        }
    }
    DBG("\r\n");
    
}
#endif

void ReadSettingsFromFlash(int mode)
{
#ifndef PCMODE
    unsigned char* ptr;
    unsigned char* flash;
    char t[64];
    int i;
    flash=(unsigned char*)FLASH_SETTINGS_ADDRESS;
    ptr=(unsigned char*)&settings;
    for (i=0;i<sizeof(settings);i++)
    {
        //if (mode) sprintf(t,"0x%08lx --> 0x%08lx\r\n",flash,ptr); DBG(t);
        *ptr++=*flash++;
    }
    if (mode) DumpRawSettings();
#endif    
}
void SaveSettingsToFlash(void)
{
#ifndef PCMODE
     FlashSetup();
     while(1)
     {
         if (FlashWriteAccess(FLASH_SETTINGS_ADDRESS,(unsigned char*)&settings,sizeof(settings))) return;
     }
#endif
}


void GetRunningLapTime(int mode)
{
    char txt[64];
#ifdef PCMODE
	static unsigned long full = 0;
	global.running_lap_time = full++;
#else
	unsigned long full = 0;
	// grab the lap time here
	full = TMR3;
	full = full << 16;
	full |= TMR2;
            
            
    if (global.flash_play==0)
    {
        if (global.lapnumber!=0)
        {
            global.running_lap_time = AimScaleDouble(INTERNAL_LAP_TIMER, full);
        }
    }
#endif

    // sprintf(txt,"00 %ld\r\n",full); DBG(txt);
    if (mode==1)
    {
     sprintf(txt,"RunningTIme=%f\r\n", AimScaleDouble(INTERNAL_LAP_TIMER, full)); DBG(txt);
    }
    
}
// 32-bit lap timer
// update global.lap_time as seconds such as 5.432 seconds
// or 127.882 seconds
void LapTimer(int setup,int reset)
{
    char txt[50];
    unsigned long full=0;
	double diff;
#ifdef PCMODE
	static struct timeb start, end;
    int i = 0;
    if (setup)
	{
			ftime(&start);
			return;
	}

    ftime(&end);
    diff = (int) (1000.0 * (end.time - start.time) + (end.millitm - start.millitm));
	ftime(&start);
	global.lap_time=(double)diff/(double)1000;

#else
    if (setup)
    {
        //DBG("LapSetup\r\n");
        T2CON = 0x0;
        T3CON = 0x0;
        T2CONbits.TCKPS=7; // 1:256 prescale at 40MHz pclock rate = 156250 ticks every second which is 6.4uS per tick
        TMR2 = 0; // clear counters
        TMR3 = 0;
        PR3 = 0xffff;
        PR2 = 0xffff;
        T2CONbits.TON = 1;
        T2CONbits.T32 = 1;
        return;
    } 

    
    
    if (reset)
    {
        //DBG("LapSetup\r\n");
        // grab the lap time here
        full=TMR3;
        full=full<<16;
        full|=TMR2;
        if (global.flash_play!=0)
        {
            global.lap_time=AimScaleDouble(INTERNAL_LAP_TIMER,full);
        }
        TMR2=0;
        TMR3=0;
        gps_distance(0,0); // reset the gps distance
    }
#endif

}

// approximately 1/20th of a second timer ticks
void IncrementTimer(void)
{
	static int lc=0;
    char msg[64];
        GetRunningLapTime(0);
        lc++;
        global.accel_poll=1;
        if (global.dash_stream==1) global.dash_send++;
#ifndef PCMODE
        GenerateOBDII();
#endif
        WrLogData();
        if (global.throttle_timer!=0)   global.tenths_timer+=(double)0.1; else global.tenths_timer=0;
        if (lc>10)
         {
            lc=0;
            global.sys_timer++; // 1second
            if (global.rep_trigger==0)
            {
                global.rep_timer=0;
            }
            else
            {
                global.rep_timer++; // 1 second
                if (global.rep_trigger==2)
                {
                    if (global.rep_timer>=3) 
                    {
                        global.rep_timer=0;
                        global.rep_trigger=0;
                    }
                }
                else
                {
                    if (global.rep_timer>=6) 
                    {
                        global.rep_timer=0;
                        global.rep_trigger=0;
                    }
                }
            }
            
            //sprintf(msg,"Timer=%ld,%ld,%d\r\n",global.sys_timer,tv,lc);
            //DBG(msg);
        }
    
}   

// this is called from system_init.c
// the uart mappings changed from the original racevoice 1.7 pcb
// moving forward they all now look like the standalone unit
// this only really effects the STDBY pin for the VoiceChip
// this is referenced in drv_usart_static.c to turn on/off uart6
// which is used as GPIO in the racevoice 1.x boards
#ifdef PCMODE
unsigned char IsNewLevelBoard(void)
{
	return 1;
}

#else

unsigned char IsNewLevelBoard(void)
{
    int state=0;
    int i;
    int j;
    int follow=0;
    
    new_level_board=0;
    follow=0;
    for (i=0;i<10;i++)
    {
        state=i&1;
        for (j=0;j<100;j++) BOARD_LOOP_OUTStateSet(state);

        if (BOARD_LOOP_INStateGet()==state) follow++; else follow=0;
    }
    if (follow==10) 
    {
        new_level_board=1;
         return 1;   
    }
    
    return 0;
}
#endif



void FindBoardVersion(void)
{
    int follow=0;
    int state=0;
    int i;
    int j;
     // determine the board version
    global.board_version=RACEVOICE_CS_ORIGINAL;
    global.board_option=0;
#ifndef PCMODE
    if (new_level_board)
    {
        global.board_version=RACEVOICE_STANDALONE;
        if (MODE0StateGet()) global.board_option|=0x01;
        if (MODE1StateGet()) global.board_option|=0x02;
        if (MODE2StateGet()) global.board_option|=0x04; 
        
        if (init_was_done)
        {
            init_was_done=0;
            settings.dash_type=STANDALONE;
        }
        
        if (settings.can_terminate)
        {
            CANTERMOn();
        }
        else
        {
            CANTERMOff();
        }
    }
#endif
//    while(1) asm("nop");
    
    
}
void InitSettings(void)
{
    int reset=0;
    int i;
    int j;
    
    LapTimer(1,0);
    ReadSettingsFromFlash(1);
    global.flash_play=0;
    
    
#ifdef BETA
#ifdef JIM
    //[59]    [89]   [119] as reminders of predicting MPH to Upshift.
    settings.mph_trigger[0]=59;
    settings.mph_trigger[1]=89;
    settings.mph_trigger[2]=119;
#endif
#endif
    
    if (settings.valid!=VALID_KEY) reset=1;
    
#ifdef PCMODE
	reset = 1;
#endif
    
    if (reset) settings.trace_level=0x00;
    
    
    // force the tps high threshold to be 95% at least if it hasn't been set
    // this way ... we give extended feedback once the driver is back on the straight away
    if (settings.tps_high==0 || reset) settings.tps_high=TPS_HIGH_DEFAULT;
	if (settings.tps_low==0 || reset) settings.tps_low=30;
    if (settings.minmph_threshold==0 || reset) settings.minmph_threshold=10;
    if (settings.mph_trigger_delta==0 || reset) settings.mph_trigger_delta=15;
    if (settings.corner_lateral_g_trigger==0 || reset) settings.corner_lateral_g_trigger=0.7;
    
    
    // rpm notice is the number of times to announce the up/down shift message
    // should be merged with shift_announce flag
	if (settings.rpm_notice==0 || reset) settings.rpm_notice=1;
     
    
    if (reset)
    {
        memset(settings.unit_name,0,sizeof(settings.unit_name));
        sprintf(settings.unit_name,"NONE\0");
        settings.checker_lattitude=0;
        settings.checker_longitude=0;
        
        for (i=0;i<MAX_SEGMENTS;i++)
        {
            
            settings.segment_start_lat[i]=0;
            settings.segment_start_lng[i]=0;
            settings.segment_stop_lat[i]=0;
            settings.segment_stop_lng[i]=0;
            settings.segment_enable[i]=0;
            settings.split_enable[i]=0;
            settings.split_lat[i]=0;
            settings.split_lng[i]=0;

            
        }
        
        settings.rpm_overrev=8000;
        settings.rpm_high=6620;
        settings.rpm_low=4000;
        settings.rpm_oil_threshold=1500;
        settings.oil_low=10;
        settings.temp_high_f=215;
        settings.volts=11.5;
        settings.lateral_g_high_trigger=1.0;
        settings.linear_g_high_trigger=1.0;
        
        settings.overrev_announce=1;
        settings.upshift_announce=1;
        settings.downshift_announce=0;
        settings.oil_announce=1;
        settings.temp_announce=1;
        settings.volts_announce=1;
        settings.lateral_gforce_announce=0; 
        settings.linear_gforce_announce=0; 
        settings.mph=105;
        memset(settings.mph_range,0,sizeof(settings.mph_range));
        settings.mph_announce=0;
        settings.trackindex=0;
        memset(settings.trackname,0,sizeof(settings.trackname));
        sprintf(settings.trackname,"NO-SELECTION");
        
        settings.lap_announce=0;
        settings.coach_mode=0;
        settings.can_trace=0;
        settings.shift_tone=0;
        settings.shift_tone_hz=900;
        settings.shift_tone_duration=4; // 100ms

        settings.wheel_speed_brake_threshold=100;
        settings.wheel_speed_delta=0;
        settings.wheel_speed_enable=0;
        
        settings.voice_volume=5; // default rc8660 setting
        settings.voice_speed=5;  // default rc8660 setting
        settings.voice_pitch=50; // default rc8660 setting
        settings.voice_type=0;   // default rc8660 setting 
        settings.gpswindow=1; // standard
        settings.brake_tone_psi_low=500;
        settings.brake_tone_psi_high=600;
        settings.brake_tone_hz=1500;
        settings.brake_tone_duration=50; // half a second
        settings.brake_tone_enable=0;
        settings.tire_low_psi[0]=0;
        settings.tire_high_psi[0]=0;
        
#ifdef STEVE
        settings.tire_low_psi[0]=18;
        settings.tire_high_psi[0]=25;
#endif
        
        for (i=0;i<MAX_PHRASE;i++)
        {
            settings.phrase[i][0]='B';
            settings.phrase[i][1]='L';
            settings.phrase[i][2]='A';
            settings.phrase[i][3]='N';
            settings.phrase[i][4]='K';
            settings.phrase[i][5]='0'+i;
            settings.phrase[i][6]=0;
            settings.phrase_control[i]=0;
        }
        settings.dash_variant=0;
        settings.dash_type=AIM;
        settings.baud_rate=_250K_;
        settings.log_enabled=0; 
        settings.can_listen=0;
        settings.can_terminate=1;

#ifdef FORCE_OBDII
        settings.dash_type=OBD_II;
        settings.tps_high=TPS_OBD_HIGH_DEFAULT;
        settings.baud_rate=_500K_;
#endif
       for (i=0;i<NUMBER_OF_CAN_IDS;i++)
       {
            settings.canconfig[i].can_id=0;
            settings.canconfig[i].resource=0;
            settings.canconfig[i].type=0;
            settings.canconfig[i].offset=0;
            settings.canconfig[i].mult=0;
           for (j=0;j<8;j++)
           {
            settings.canconfig[i].mask[j]=0;
           }
       }
        if (global.board_version==RACEVOICE_STANDALONE) settings.log_enabled=1;

#ifdef FORCE_CUSTOM_CAN
        settings.dash_type=CUSTOM_CAN;
        sprintf(settings.canconfig_name,"SRF3-NATIVE\0");
#endif
        
        // test values
#ifdef SMARTY_CAM
        settings.baud_rate=_1000K_;
        settings.dash_type=SMARTY;
#endif

#ifdef STEVE
            settings.dash_type=MOTEC;
            settings.baud_rate=_250K_;
            settings.voice_volume=9;
            
            settings.dash_type=OBD_II;
            settings.baud_rate=_500K_;
#endif
            
#ifdef BETA
        #ifdef JIM
            settings.mph=105;
            settings.dash_type=AIM;
            settings.mph_announce=1;
        #endif
           
            //settings.lateral_gforce_announce=1; 
            //settings.coach_mode=1;
            //settings.mph=95;
            settings.mph_announce=1;
            settings.rpm_high=6550;
            settings.mph_range[0]=60;
            settings.mph_range[1]=70;
            settings.mph_range[2]=80;
            settings.mph_range[3]=90;
        
#endif   
   

            init_was_done=1;
#ifdef PCMODE
            settings.dash_type=AIM;

#endif
    }
    
    
    global.can_voice_speed=settings.voice_speed;
    global.can_voice_pitch=settings.voice_pitch;
    global.can_voice_type=settings.voice_type;
   
    //if (reset) settings.dash_type=AIM;
    
    if (reset)
    {
        settings.valid=VALID_KEY;
        SaveSettingsToFlash();
    }

}



#ifndef PCMODE

void DemoLoop(void)
{
    static int state=1;
    char msg[50];
    
#if(0)
    if (global.sys_timer>3)
    {
        global.sys_timer=0;
    }
    else return;
#endif
    
   sprintf(msg,"DemoLoop %d,%f\r\n",state,global.sys_timer);
      DBG(msg);

    global.oil_pressure_psi=7;
    global.engine_temp_f=235;
    global.volts=11.6;
    
    switch(state)
    {
        case 1:
             global.mph=20;
             SpeakControl(VOICE,1);  
             SpeakControl(PITCH,80);  
             SpeakControl(FREQ,50);  
             SpeakControl(TONE,0); 
             SpeakControl(REVERB,2); 
            //state=11; break;
            Announce(OIL);
            state=2;
            break;
        case 2:
            Announce(TEMP);
            state=3;
            break;
        case 3:
            Announce(VOLTS);
            state=4;
            break;
        case 4:
            Announce(UP);
            state=5;
            break;
        case 5:
            Announce(DOWN);
            state=6;
            break;
        case 6:
            Announce(OVERREV);
            state=7;
            break;
        case 7:
            global.latched_minspeed=73;
            Announce(MINIMUM_SPEED);
            state=8;
            break;
        case 8:
            global.lateral_g=1.3; Announce(LATERAL);
            global.lateral_g=1.4; Announce(LATERAL);
            global.lateral_g=1.5; Announce(LATERAL);
            global.lateral_g=1.6; Announce(LATERAL);
            global.lateral_g=1.5; Announce(LATERAL);
            global.lateral_g=1.5; Announce(LATERAL);
            global.lateral_g=1.2; Announce(LATERAL);
            state=9;
            break;
        case 9:
            global.delta_time=.85;
            Announce(SPLIT);
            state=10;
            break;
        case 10:
            global.delta_time=-.32;
            Announce(SPLIT);
            state=11;
            break;
            
        case 11:
            global.mph=96; Announce(SPEED);
            global.mph=99; Announce(SPEED);
            global.mph=105; Announce(SPEED);
            global.mph=110; Announce(SPEED);
            global.mph=114; Announce(SPEED);
            global.mph=115; Announce(SPEED);
            global.mph=115; Announce(SPEED);
            Announce(UP);
            Announce(UP);
            global.mph=116; Announce(SPEED);
            global.mph=118; Announce(SPEED);
            global.mph=120; Announce(SPEED);
            state=12;
            break;
        case 12:
            global.linear_g_triggered=0.8;
            Announce(LINEAR);
            state=13;
            break;
            
        case 13: DBG("DONE!\r\n"); state=14; break;
        case 14: while(1) asm("nop"); break;
        
        defualt:state=1; break;
    }
}

#endif



void PlaySingleDigit(int val,int trace)
{
        if (val==0) PlayAudio(7,trace);  // ohh
        if (val==1) PlayAudio(8,trace);   // one
        if (val==2) PlayAudio(9,trace);
        if (val==3) PlayAudio(10,trace);
        if (val==4) PlayAudio(11,trace);
        if (val==5) PlayAudio(12,trace);
        if (val==6) PlayAudio(13,trace);
        if (val==7) PlayAudio(14,trace);
        if (val==8) PlayAudio(15,trace);
        if (val==9) PlayAudio(16,trace);
        if (val==10) PlayAudio(17,trace);
}

void PlayTeens(int val,int trace)
{
        if (val==10) PlayAudio(17,trace);   // ten
        if (val==11) PlayAudio(18,trace);   // eleven
        if (val==12) PlayAudio(19,trace);
        if (val==13) PlayAudio(20,trace);
        if (val==14) PlayAudio(21,trace);
        if (val==15) PlayAudio(22,trace);
        if (val==16) PlayAudio(23,trace);
        if (val==17) PlayAudio(24,trace);
        if (val==18) PlayAudio(25,trace);
        if (val==19) PlayAudio(26,trace);
}

void PlayTen(int val,int trace)
{
        if (val==20) PlayAudio(27,trace);
        if (val==30) PlayAudio(28,trace);
        if (val==40) PlayAudio(29,trace);
        if (val==50) PlayAudio(30,trace);
        if (val==60) PlayAudio(31,trace);
        if (val==70) PlayAudio(32,trace);
        if (val==80) PlayAudio(33,trace);
        if (val==90) PlayAudio(34,trace);
}

void PlayAudioInteger(int val, int trace)
{
    char tval[5];
	int triple = 0;
	memset(tval,0,sizeof(tval));
    sprintf(tval,"%03d\0",val);
    
    if (val>=100)
    {
        PlayAudio(8,trace);    // Say "one"
        val=val-100;
        triple=1;
    }
    
    if (val<=10)
    {
       if (tval[1]=='0' && triple)
        {
           // Say "ohhh"
            PlayAudio(7,trace);  
        }
        // say single digit
        PlaySingleDigit(val,trace);
    }
    
    if (val>=11 && val<=19)
    {
        PlayTeens(val,trace);
    }
    
    if (val>=20 && val<=99)
    {
        if (tval[1]=='2') PlayTen(20,trace); // say twenty
        if (tval[1]=='3') PlayTen(30,trace);
        if (tval[1]=='4') PlayTen(40,trace);
        if (tval[1]=='5') PlayTen(50,trace);
        if (tval[1]=='6') PlayTen(60,trace);
        if (tval[1]=='7') PlayTen(70,trace);
        if (tval[1]=='8') PlayTen(80,trace);
        if (tval[1]=='9') PlayTen(90,trace);
        
        if (tval[2]=='1') PlaySingleDigit(1,trace);
        if (tval[2]=='2') PlaySingleDigit(2,trace);
        if (tval[2]=='3') PlaySingleDigit(3,trace);
        if (tval[2]=='4') PlaySingleDigit(4,trace);
        if (tval[2]=='5') PlaySingleDigit(5,trace);
        if (tval[2]=='6') PlaySingleDigit(6,trace);
        if (tval[2]=='7') PlaySingleDigit(7,trace);
        if (tval[2]=='8') PlaySingleDigit(8,trace);
        if (tval[2]=='9') PlaySingleDigit(9,trace);
    }
    
    
}

void PlayAudio(int slot,int trace)
{
    char cmd[32];

#ifndef PCMODE
	SpeechByte(1);
#endif
    if (trace)
    {
        sprintf(cmd,"%d&",slot);
        DBG("Will play [");
        DBG(cmd);
        DBG("]\r\n");
    }
	#ifndef PCMODE
	SpeechString(cmd,1,1);
	#endif
	if (trace)
    {
        DBG("DONE!\r\n");
    }
}

#ifndef PCMODE
void EraseAudio(void)
{
    char cmd[32];
    DBG("SendErase...\r\n");
	SpeechByte(1);
	SpeechByte('2');
	SpeechByte('2');
	SpeechByte('3');
	SpeechByte('W');
    DBG("DONE!\r\n");
}

void InitAudio(void)
{
    int busy=1;
    int found=0;
    DBG("SendInit...\r\n");
	SpeechByte(1);
	SpeechByte('2');
	SpeechByte('1');
	SpeechByte('4');
	SpeechByte('W');
    while(busy)
    {
    // wait until we see the character we are asking for
        found=GetChSp();
        if (found>0)
        {
            if ((unsigned char)found==0x06) busy=0;
        }
    }
    DBG("DONE!\r\n");
	SpeechByte('4');
	SpeechByte('\r');
}

#endif



void ProcessTrackFeedback(void)
{
#ifdef PCMODE
	global.gps_error = 0;
	global.canrx = CAN_RX_THRESHOLD * 2;
#endif
	if (global.mph>settings.minmph_threshold && global.gps_error==0 &&  (global.board_version==RACEVOICE_STANDALONE || global.canrx>=CAN_RX_THRESHOLD))
    {
        if (!HandleLateral())
        {
            HandleRPM();
            HandleMPH(0);
            HandleLinear();
        }
        else
        {
            HandleMPH(1); // announce mph during a segment if enabled
        }
        HandleWheelSpeed();
        HandleBrakeTone();
        HandlePhrases();
        HandleSegments();
        HandleLap();
    }
    //HandleBrakeTone();
}


#ifndef PCMODE

// send a debug text message on the USB Serial Port
int GetChSp(void)
{
    int ch=-1;
    if (!DRV_USART_ReceiverBufferIsEmpty(appData.handleUSART1))
    {
        ch=DRV_USART_ReadByte(appData.handleUSART1);
    }
    return ch;
}

int WaitSpeech(unsigned char ch)
{
    int found=0;
    char txt[32];
    // wait until we see the character we are asking for
        found=GetChSp();
        if (found>0)
        {
            //sprintf(txt,"SpeechRX=%c\r\n",found);
            //DBG(txt);
            if ((unsigned char)found==ch) return 1;
        }
        //DBG("wait..\r\n");
    return 0;
    
}


void SendStop(void)
{
    SpeechByte(1);
    SpeechByte(0x18);
    
}
void SpeechByte(unsigned char ch)
{
    unsigned long j=0;
    char tt[10];
    memset(tt,0,sizeof(tt));
    tt[0]=ch;
    tt[1]='\r';
    tt[2]='\n';
#ifdef NO_VOICE
    return;
#endif          
    while(1)
    {
       // check CTS line from the Speech Chip to see if we can send it data
 
       if(SCTSStateGet()==0)
       {
           // make sure we don't have a byte already in the UART output buffer
        //DBG(tt);
        if(!DRV_USART_TransmitBufferIsFull(appData.handleUSART1))
        {
              // send a byte to the Speech CHIP
              DRV_USART_WriteByte(appData.handleUSART1,  ch);
              break;
        }
       }
       else
       {
           //DBG("busy..\r\n");
           
       }
    } 
    
}


int SpeechString(char* txt,int cr,int priority) 
{
    unsigned long i=0;
    static int busy=0;
    static long lt=0;
    static int first=1;
    long dt=0;
#ifdef NO_VOICE
    return 1;
#endif    
    if (cr && busy)
    {
        // are we done speaking any previous message?
        if (priority)
        {
            // wait until any message has completed
            // then allow the current priority message to be spoken
             while(!WaitSpeech('t')) 
             {
                 HandleSPIFlash();
                 busy=1;
             }
            busy=0;
        }
        else
        {
                if (WaitSpeech('t')) busy=0; 
                if (busy) return 0;
        }
    }
    
    
    if (priority & 0x2) PushSpeechBuffer(0); // clear
    while(1)
    {
        SpeechByte(*(txt+i));
        if (priority & 0x2) PushSpeechBuffer(*(txt+i));
        i++;
        if (i==strlen(txt)) break;
    }
    if (priority & 0x2) PushSpeechBuffer(0xaa); // commit
   
    if (cr)
    {
        
        SpeechByte('\r');
        // wait for the output has started message
        while (!WaitSpeech('s'))
        {
            HandleSPIFlash();
            busy=1; 
        }
    }
    return 1;
    
}



// send a character message on the USB Serial Port
void SendDBGByte(unsigned char ch)
{
    while(1)
    {
        if(!DRV_USART_TransmitBufferIsFull(appData.handleUSART0))
                {
                    DRV_USART_WriteByte(appData.handleUSART0,ch);
                    break;
                    
                }
    } 
    
}
void DBG(char* txt) 
{
    int i;
    for (i=0;i<strlen(txt);i++)
    {
        if (global.btlink) B_UTX(*(txt+i));
        //else
            SendDBGByte(*(txt+i));
    }
    
}

void SendCAN(void)
{
    static int loop=0;
    int addr=0;
    char t[32];

    sprintf(t,"SendCan=%d\r\n",loop);
    DBG(t);

    memset(canTXB,0,sizeof(canTXB));
    switch(loop)
    {
        case 0:
            addr=CAN_TIRE_LF;
            canTXB[0]=110;
            canTXB[1]=120;
            canTXB[2]=130;
            break;
        case 1:
            addr=CAN_TIRE_RF;
            canTXB[0]=111;
            canTXB[1]=121;
            canTXB[2]=131;
            break;
        case 2:
            addr=CAN_TIRE_LR;
            canTXB[0]=112;
            canTXB[1]=122;
            canTXB[2]=132;
            break;
        case 3:
            addr=CAN_TIRE_RR;
            canTXB[0]=113;
            canTXB[1]=123;
            canTXB[2]=133;
            break;
    }
    
    if (DRV_CAN_ChannelMessageTransmit(appData.handleCAN0,CAN_CHANNEL14,addr,  sizeof(canTXB),  canTXB) == true)
            {
                global.cantx++;
            }
            else
            {
                global.cantx_fail++;
            }
    
    loop++;
    if (loop>=4) loop=0;
                
    
}

int GotCAN(void)
{
    if (global.can_irq)
    {
        global.can_irq=0;
        return 1;
    }
    
    return 0;
}


// called from the irq routine
void CANRX_IRQ(void)
{

    static int first_address=0x50;
    static int channel=0;
    int valid=0;
    int address=0;
    int i;
    char it[10];
    static unsigned long ci=0;
   
    
    //DBG("IRQ\r\n");
   
    
    switch(settings.dash_type)
    {
        case STANDALONE:
                    global.can_irq=1;
            break;
            
        case OBD_II:
                address=0x7E8;
                if (DRV_CAN_ChannelMessageReceive(appData.handleCAN0,channel, address,RXB_SIZE,canRXB)==true)
                {
                    valid=1;
                    if (fw_update==0) FlashCanLED();
                    global.canrx++;
                }
                if (valid && fw_update==0) 
                {
                    ProcessOBDII();
                    global.can_irq=1;
                }   
            break;
        case CUSTOM_CAN:
                for (channel=0;channel<NUMBER_OF_CAN_IDS;channel++)
                {
                    address=global.CAN_IDS[channel];
                    valid=0;
                    if (DRV_CAN_ChannelMessageReceive(appData.handleCAN0,channel, address,RXB_SIZE,canRXB)==true)
                    {
                        valid=1;
                        if (fw_update==0) FlashCanLED();
                        global.canrx++;
                    }
                    if (valid && fw_update==0) 
                    {
                        ProcessCustomCan();
                        global.can_irq=1;
                     }   
                }
            break;
        case SMARTY:
        for (channel=0;channel<6;channel++)
        {
            valid=0;
            if (channel==0) address=SMARTY_GPS;
            if (channel==1) address=SMARTY_1056;
            if (channel==2) address=SMARTY_1057;
            if (channel==3) address=SMARTY_1058;
            if (channel==4) address=SMARTY_1059;
            if (channel==5) address=SMARTY_1060;
            if (DRV_CAN_ChannelMessageReceive(appData.handleCAN0,channel, address,RXB_SIZE,canRXB)==true)
            {
                valid=1;
                if (fw_update==0) FlashCanLED();
                global.canrx++;
            }
            if (valid && fw_update==0) 
            {
                ProcessCanRX();
                global.can_irq=1;
            }
        }
        break;
    
    case VBOX:
        for (channel=0;channel<4;channel++)
        {
            valid=0;
            if (channel==0) address=VBOX_LAT;
            if (channel==1) address=VBOX_LNG_VEL;
            if (channel==2) address=VBOX_GFORCE;
            if (channel==3) address=VBOX_LAP;
            if (DRV_CAN_ChannelMessageReceive(appData.handleCAN0,channel, address,RXB_SIZE,canRXB)==true)
            {
                valid=1;
                if (fw_update==0) FlashCanLED();
                global.canrx++;
            }
            if (valid && fw_update==0) 
            {
                ProcessCanRX();
                global.can_irq=1;
            }
        }
        break;
    
    default:
        for (channel=0;channel<16;channel++)
        {
            valid=0;
            address=(first_address+channel);
            if (channel==11) address=AUTOSPORTS_A10L;
            if (channel==12) address=AUTOSPORTS_A10H;
            if (channel==13) address=CAN_TPMS;
            if (DRV_CAN_ChannelMessageReceive(appData.handleCAN0,channel, address,RXB_SIZE,canRXB)==true)
            {
                valid=1;
                if (fw_update==0) FlashCanLED();
                global.canrx++;
            }
            if (valid && fw_update==0) 
            {
                ProcessCanRX();
                global.can_irq=1;
            }
        }
        break;
    }
    

}

// LED Heartbeat Pulse
int HeartBeat(void)
{
    char mt[10];
    static int t=0;
    static unsigned long hb=0;
    hb++;
    // change pattern for firmware update
    if (fw_update)
    {
        if (hb<25000) 
        {
            GREEN_LEDOn();
            BLUE_LEDOn();        
        }
        else
        {
            GREEN_LEDOff();
            BLUE_LEDOff();
        }
        if (hb>50000) {global.hb++; hb=0;}
    }
    else
    {
        if (global.board_version==RACEVOICE_STANDALONE)
        {
            if (global.flash_record==0)
            {
                if (hb<10000)
                {
                    GREEN_LEDOn();
                }
                else 
                {
                    GREEN_LEDOff();
                }
            }
        }
        else
        {
            if (hb<1000) GREEN_LEDOn(); else GREEN_LEDOff();
        }
        if (hb>200000) {global.hb++; hb=0;}
    }
    
    if (hb==0) return 1; else return 0;

}

#endif



long Scale(unsigned short address, long value)
{
        if (settings.dash_type==MOTEC) return MotecScale(address,value);
        if (settings.dash_type==AUTOSPORTS || settings.dash_type==AIM || settings.dash_type==SMARTY) return AimScale(address,value);
        if (settings.dash_type==VBOX) return VboxScale(address,value);
        return 0;
    
}

double ScaleDouble(unsigned short address, long value)
{
    if (settings.dash_type==MOTEC) return MotecScaleDouble(address,value);
    if (settings.dash_type==AUTOSPORTS || settings.dash_type==AIM || settings.dash_type==SMARTY) return AimScaleDouble(address,value);
    if (settings.dash_type==VBOX) return VboxScaleDouble(address,value);
    return 0;
    
}

// CSV output one line of real-time values
void ShowLiveValues(void)
{
    static char msg[128];
    memset(msg,0,sizeof(msg));
    // dashtype, tps, rpm, mpg, brake, temp, pressure, volts, lattitude, longitude
    sprintf(msg,"\r\nLIVETRACE,%d,%d,%d,%d,%d,%d,%,%d,%f,%f,%f\r\n",settings.dash_type,global.tps,global.rpm,global.mph,global.brake_front_psi,global.engine_temp_f,global.oil_pressure_psi,global.volts,global.gps_lat,global.gps_lng);
    DBG(msg);
}


// Process the can RX Message
void ProcessCanRX(void)
{
    unsigned short address;
    unsigned short original_address;
    unsigned long value;
    unsigned short value23;
    unsigned long value2;
    unsigned long value3;
    unsigned long value4;
    unsigned long value32b1;
    unsigned long value32b2;
    double dvalue=0;
    double dvalue2=0;
    static int step=0;
    int tyre;
    int i;
    int x;
    static char msg[128];
    static unsigned char flip=0;

    
    original_address=address=canRXB[0];
    if (address==CAN_DEBUG_FLAG)
    {
     value=canRXB[4] | canRXB[3]<<8 | canRXB[2]<<16 | canRXB[1]<<24;
    }
    else
    {
        
        if (settings.dash_type==SMARTY)
        {
            
             value32b1=canRXB[5]<<24 | canRXB[4]<<16 | canRXB[3]<<8 | canRXB[2];
            value=canRXB[2]<<8 | canRXB[1];
            if (value & 0x8000) value|=0xffff0000; // sign extended

            value2=canRXB[4]<<8 | canRXB[3];
            if (value2 & 0x8000) value2|=0xffff0000; 

            value3=canRXB[6]<<8 | canRXB[5];
            if (value3 & 0x8000) value3|=0xffff0000; 
            
            value4=canRXB[8]<<8 | canRXB[7];
            if (value4 & 0x8000) value4|=0xffff0000; 


        }
        else
        {
             value=canRXB[1]<<8 | canRXB[2];
            //sprintf(msg,"value1=0x%08x\r\n",value); DBG(msg);
            if (value & 0x8000) value|=0xffff0000; // sign extended

            value23=canRXB[2]<<8 | canRXB[3];

            value2=canRXB[3]<<8 | canRXB[4];
            //sprintf(msg,"value2=0x%08x\r\n",value2); DBG(msg);
        
            value32b1=value<<16 | value2;
            if (value2 & 0x8000) value2|=0xffff0000; 

            value3=canRXB[5]<<8 | canRXB[6];
            // sprintf(msg,"value3=0x%08x\r\n",value3); DBG(msg);
            if (value3 & 0x8000) value3|=0xffff0000; 
        
            value4=canRXB[7]<<8 | canRXB[8];
            //sprintf(msg,"value1=0x%08x\r\n",value4); DBG(msg);
            value32b2=value3<<16 | value4;
            if (value4 & 0x8000) value4|=0xffff0000; 
            //sprintf(msg,"32b1=0x%08lx 32b2=0x%08lx\r\n",value32b1,value32b2); DBG(msg);
        
        }
       
    }
    
   
    // now decode our message
    
        step++;
       if ((global.can_trace && step>=10))
       {
           step=0;
           memset(msg,0,sizeof(msg));
           sprintf(msg,"GPS=[%f,%f] RPM=%d MPH=%d Temp=%d Oil=%d Tps=%d Volts=%.2f AcX=%.2f AxY=%.2f DeltaTime=%.3f\r\n",global.gps_lat,global.gps_lng,global.rpm,global.mph,global.engine_temp_f,global.oil_pressure_psi,global.tps,global.volts,global.linear_g,global.lateral_g,global.delta_time); DBG(msg);
           DBG(msg);
           //sprintf(msg,"GPS=[%f,%f]\r\n",global.gps_lat,global.gps_lng);
           DBG(msg);
           
        }
        
        if (global.can_trace==2)
        {
            sprintf(msg,"Addr=%d \0",address); DBG(msg);
            for (i=0;i<8;i++)
            {
                sprintf(msg," 0x%02x \0",canRXB[1+i]);
                DBG(msg);
            }
            DBG("\r\n");
        }
     //   sprintf(msg,"Addr=0x%02x\r\n",address); DBG(msg);
    switch(address)
    {

            
         
    //CAN ID=0x0420 RX RAW= fe cf 0d 00 fe cf fe cf  -- 1056
    //CAN ID=0x0421 RX RAW= fe cf fe cf fe cf fe cf  -- 1057
    //CAN ID=0x0422 RX RAW= 97 ff fe cf ff ff fe cf  -- 1058
    //CAN ID=0x0423 RX RAW= fe cf fe cf ad ff d9 ff  -- 1059
    //CAN ID=0x0424 RX RAW= fe cf 7d 00 04 00 fe cf  -- 1060
            
        // SmartyCam Emulation Mode
        case SMARTY_1056:
            global.rpm=Scale(SCALE_RPM,value);
            if (global.rpm>settings.rpm_overrev && global.rpm<(settings.rpm_overrev<<1))
            {
                global.overrev=global.rpm;
            }
            if (settings.dash_variant==0)
                global.mph=Scale(SCALE_MPH_D10_KM,value2); // value is in KPH, so say it in MPH
            else
                global.mph=Scale(SCALE_MPH_D10_MM,value2); // value is in MPH, so say it in MPH
            global.engine_temp_f=Scale(SCALE_TEMP_C,value4);
            HandleSegmentsIRQ();
            break;
        case SMARTY_1057:
            // 
            x=Scale(SCALE_OIL,value2);
            if (x<100) x=OIL_NOT_MEASURED;
            global.oil_pressure_psi=x;
            break;

        case SMARTY_1058:
            
            global.brake_front_psi=(SCALE_BAR_TO_PSI,value);
            global.tps=Scale(SCALE_TPS,value2);
            if (global.tps>=settings.tps_high) global.tps_tickh++; else global.tps_tickh=0;
            if (global.tps<=settings.tps_low) global.tps_tickl++; else global.tps_tickl=0;
            
            break;

        case SMARTY_1059:
            global.ex_gforce=1;
            global.lateral_g=ScaleDouble(CAN_LAT_G,value3);
            global.lateral_g_raw=ScaleDouble(SCALE_LAT_G_RAW,value3);
            global.linear_g=ScaleDouble(CAN_LAT_G,value4);
            global.linear_g_raw=ScaleDouble(SCALE_LAT_G_RAW,value4);
            HandleLinearIRQ();
            break;
            
        case SMARTY_1060:
            global.volts=ScaleDouble(SCALE_VOLTS,value2);
            break;
            
           // 1hz ...
        case SMARTY_GPS:
#ifdef GPS_IRQ_TIMING
            if (flip)
            {
                BLUE_LEDOn();
                flip=0;
            }
            else
            {
                BLUE_LEDOff();
                flip=1;
            }
#endif
            //115654222;RX;0x28;5;0x00;0x94;0x23;0x89;0xD2;0x00;0x00;0xD3  --> lattitude
            //104461221;RX;0x28;5;0x01;0x96;0xCF;0x4D;0x19;0x00;0x00;0xD3  --> longitude
            if (canRXB[1]==0x00)
            {
               global.ex_gps=1;
               global.gps_lng=ScaleDouble(SCALE_GPS,value32b1);
               // sprintf(msg,"RawLNG=0x%08lx   GPS=%f,%f\r\n",value32b1,global.gps_lat,global.gps_lng);
            }
            
            if (canRXB[1]==0x01)
            {
                global.gps_lat=ScaleDouble(SCALE_GPS,value32b1);
                global.gps_update=1;
               // sprintf(msg,"RawLAT=0x%08lx   GPS=%f,%f\r\n",value32b1,global.gps_lat,global.gps_lng); 
            }
             //DBG(msg);
          break;
            
        // vbox motorola format
        //https://racelogic.support/02VBOX_Motorsport/Video_Data_Loggers/VBOX_Video_HD2/VBOX_Video_HD2_User_Guide/09_-_HD2_Technical_Properties/HD2_-_CAN_Output_Format
        case VBOX_LAT:
                   global.ex_gps=1;
                   global.gps_lat=ScaleDouble(VBOX_LAT,value32b2); // 100k
                   break;
        case VBOX_LNG_VEL:
                    global.gps_lng=ScaleDouble(VBOX_LNG_VEL,value32b1); //100k
                    global.mph=Scale(SCALE_MPH,value3);  // knots
                    HandleSegmentsIRQ();
                    global.gps_update=1;
                    

                    break;
        case VBOX_GFORCE:
                    global.ex_gforce=1;
                    global.lateral_g=ScaleDouble(CAN_LAT_G,value3);
                    global.lateral_g_raw=ScaleDouble(SCALE_LAT_G_RAW,value3);
                    global.linear_g=ScaleDouble(CAN_LAT_G,value4);
                    global.linear_g_raw=ScaleDouble(SCALE_LAT_G_RAW,value4);
                    HandleLinearIRQ();
                    break;
        case VBOX_LAP:
            break;
            
            
        case CAN_WHEEL_SPEED:
                global.wheel_speed[0]=value;
                global.wheel_speed[1]=value2;
                global.wheel_speed[2]=value3;
                global.wheel_speed[3]=value4;
                HandleWheelSpeedIRQ();
            break;
        
#ifndef PCMODE
        case AUTOSPORTS_A10L:
                for (i=0;i<8;i++) HandleCustomSpeechIRQ(i,canRXB[1+i]);
                break;
        case AUTOSPORTS_A10H:
                for (i=0;i<8;i++) HandleCustomSpeechIRQ(8+i,canRXB[1+i]);
                HandleCustomSpeechIRQ(0xff,0);
                break;
#endif
        case CAN_DEBUG_FLAG: 
                value=(value&0xff00)>>8;
                if (value>=1)
                {
                    settings.dash_type=value;
                }
                break;
        
        case CAN_ENGINE_STAT1: 
            global.tps=Scale(SCALE_TPS,value);
            if (global.tps>=settings.tps_high) global.tps_tickh++; else global.tps_tickh=0;
            if (global.tps<=settings.tps_low) global.tps_tickl++; else global.tps_tickl=0;

            global.rpm=Scale(SCALE_RPM,value2);
            if (global.rpm>settings.rpm_overrev && global.rpm<(settings.rpm_overrev<<1))
            {
                global.overrev=global.rpm;
            }

            global.mph=Scale(SCALE_MPH,value3);

            global.brake_front_psi=Scale(SCALE_BRAKE,value4);
            break;

            
        case CAN_ENGINE_STAT2: 
            global.engine_temp_f=Scale(SCALE_TEMP,value);
            global.oil_pressure_psi=Scale(SCALE_OIL,value2);
            global.volts=ScaleDouble(SCALE_VOLTS,value3);
            break;

        case CAN_GPS:
#ifndef DISABLE_CANSENSOR
            global.ex_gps=1;
            global.gps_lat=ScaleDouble(SCALE_GPS,value32b1);
            global.gps_lng=ScaleDouble(SCALE_GPS,value32b2);
            //sprintf(msg,"RAWGPS=0x%08x,0x%08x   GPS=%f,%f\r\n",value32b1,value32b2,global.gps_lat,global.gps_lng); DBG(msg);
            HandleSegmentsIRQ();
            global.gps_update=1;
#endif
            break;
            
        case CAN_VOICE_SETTING:
            global.can_voice_speed=canRXB[1];
            global.can_voice_pitch=canRXB[2];
            global.can_voice_type=canRXB[3];
            break;
            
        case CAN_PHRASES:
            //sprintf(msg,"Phrase=0x%02x\r\n",global.phrase_triggers); DBG(msg);
            // discrete message triggers for one trigger per byte frame, useful for aim/etc
            global.phrase_triggers=0;
            // only allowed for native Aim or Motec
            if (settings.dash_type==AIM || settings.dash_type==MOTEC || settings.dash_type==AUTOSPORTS)
            {
                if (canRXB[1]==0x01) global.phrase_triggers|=0x01;
                if (canRXB[2]==0x01) global.phrase_triggers|=0x02;
                if (canRXB[3]==0x01) global.phrase_triggers|=0x04;
                if (canRXB[4]==0x01) global.phrase_triggers|=0x08;
                if (canRXB[5]==0x01) global.phrase_triggers|=0x10;
                if (canRXB[6]==0x01) global.phrase_triggers|=0x20;
                if (canRXB[7]==0x01) global.phrase_triggers|=0x40;
                if (canRXB[8]==0x01) global.phrase_triggers|=0x80;
            }
            break;
            
        case CAN_TPMS:
            tyre=canRXB[1];
            if (tyre>=1 && tyre<=4)
            {
                global.tire_pressure[tyre]=(int)ScaleTyre(0,value23);
                global.tire_temperature[tyre]=(int)ScaleTyre(1,canRXB[4]);
            }
            break;
        case CAN_LAT_G: 
#ifndef DISABLE_CANSENSOR
            global.ex_gforce=1;
            global.lateral_g=ScaleDouble(CAN_LAT_G,value);
            global.lateral_g_raw=ScaleDouble(SCALE_LAT_G_RAW,value);
            global.linear_g=ScaleDouble(CAN_LAT_G,value2);
            global.linear_g_raw=ScaleDouble(SCALE_LAT_G_RAW,value2);
            HandleLinearIRQ();
#endif
            break;

        case CAN_LAP_STATUS:
            dvalue=ScaleDouble(SCALE_LAPTIME,value4); //pos4
            global.delta_time=dvalue;
            break;

		default:break; 
           
       // default: sprintf(msg,"????\r\n "); break;
            
    }
    
    
}

#ifndef PCMODE

int GetCh(void)
{
    int ch=-1;
    unsigned long st=0;
        char dt[20];
    if (!DRV_USART_ReceiverBufferIsEmpty(appData.handleUSART0))
    {
        ch=DRV_USART_ReadByte(appData.handleUSART0);
    }
    else
    {
        st=U5STA;
        if (st & 0x8)
        {
            // parity error
            U5STACLR=0x8;
            //DBG("Parity Error!\r\n");
        }
        if (st & 0x4)
        {
            // overrun error
            U5STACLR=0x4;
            //DBG("Overrun Error!\r\n");
        }
        if (st & 0x2)
        {
            // framing error
            U5STACLR=0x2;
            //DBG("Framing Error!\r\n");
        }
    }
    return ch;
}

void Prompt(void)
{
    
    DBG("\r\nSHELL>\0");
}


void InitSpeech(void)
{
#ifdef NO_VOICE
        DBG("Speech Core Disabled...");
    return;
#endif
        DBG("Reset Speech Core...");
        if (new_level_board)   
        {
            DBG("NewLevel...");
            SSTBY_NEWOff(); 
            SRESET_NEWOff();
        }
        else
        {
            SSTBYOff(); 
            SRESETOff();
        }
        DBG("...");
        if (new_level_board)   SSTBY_NEWOn(); else   SSTBYOn(); 
        DBG("...");
        if (new_level_board) SRESET_NEWOn(); else  SRESETOn();
        DBG("...Ready!\r\n");
        
        DBG("Configuring....1");
        SpeakControl(VOLUME,(settings.voice_volume&0xf)); // max
        DBG("Configuring....2");
        SpeakControl(SETPOR,0x91);
        DBG("...Done!\r\n");
    
}

void SetVoiceSpeed(int error)
{
    if (error)
    {
        SpeakControl(VOICESPEED,5);  
        SpeakControl(VOICE,3);  
        SpeakControl(PITCH,50);
    }
    else
    {
        SpeakControl(VOICESPEED,(settings.voice_speed&0xf));  
        SpeakControl(VOICE,(settings.voice_type&0xf));  
        SpeakControl(PITCH,(settings.voice_pitch));  
    }
    
}
void SpeakVersion(int talk)
{
    char msg[64];
    int i;
    
    DBG("\r\n");
    
    
    if (global.board_version==RACEVOICE_STANDALONE)
    {
        if (talk==0)
        {
            sprintf(msg,"Race Voice-SA,  Version,  %s, %d, %d\0",VERSION,global.board_version,global.board_option); 
        }
        else
        {
            sprintf(msg,"Race Voice,  Version,  %s\0",VERSION); 
        }
    }
    else
    {
        sprintf(msg,"Race Voice,  Version,  %s\0",VERSION); 
    
    }
    DBG(msg);
    DBG("\r\n");
    
#ifdef NO_VOICE
    return;
#endif
    if (talk) 
    {
        
        if (talk==2)
        {
            Speak(msg,3);
            return;
        // speak at current volume and speed settings
        }
        SpeakControl(VOICESPEED,4);  
        SpeakControl(VOICE,2);  
        Speak(msg,3);
        SpeakControl(VOICESPEED,(settings.voice_speed&0x0f));  
        SpeakControl(VOICE,(settings.voice_type&0xf)); 
        SpeakControl(PITCH,settings.voice_pitch);
    }
}

void ShowSettings(void)
{
   char t[255];
   char tt[32];
   int i;
   int j;
   int inuse=0;
   for (i=0;i<5;i++)   DBG("\r\n");
   if (global.board_version==0)
   {
       sprintf(t,"Race Voice,  Version,  %s\r\n",VERSION);  DBG(t);
   }
   else
   {
       if (global.board_version==RACEVOICE_STANDALONE)
       {
             sprintf(t,"Race Voice-SA,  Version,  %s\r\n",VERSION);  
       }
       else
       {
             sprintf(t,"Race Voice-SA-Beta,  Version,  %s\r\n",VERSION);  
       }
       DBG(t);
   }
   DBG("--- Global Settings ---\r\n");
   sprintf(t,"Unit Name = %s\r\n",settings.unit_name); DBG(t);
   if (global.board_version==RACEVOICE_STANDALONE)
   {
       sprintf(t,"Bluetooth Address = %s %s\r\n",global.bta,global.btname); DBG(t);
   }
   sprintf(t,"RPM Overrev Threshold = %d\r\n",settings.rpm_overrev); DBG(t);
   sprintf(t,"RPM High Threshold = %d\r\n",settings.rpm_high); DBG(t);
   sprintf(t,"RPM Low Threshold = %d\r\n",settings.rpm_low); DBG(t);
   sprintf(t,"RPM Oil Threshold = %d\r\n",settings.rpm_oil_threshold); DBG(t);
   sprintf(t,"Oil Pressure Low Threshold = %d\r\n",settings.oil_low); DBG(t);
   sprintf(t,"Temperature High Threshold = %d\r\n",settings.temp_high_f); DBG(t);
   sprintf(t,"Voltage Low Threshold = %f\r\n",settings.volts); DBG(t);
   sprintf(t,"LatGForce Threshold = %f\r\n",settings.lateral_g_high_trigger); DBG(t);
   sprintf(t,"LinGForce Threshold = %f\r\n",settings.linear_g_high_trigger); DBG(t);
   sprintf(t,"WheelLockBrake Threshold = %d\r\n",settings.wheel_speed_brake_threshold); DBG(t);
   sprintf(t,"WheelLockSpeed Threshold = %d\r\n",settings.wheel_speed_delta); DBG(t);
   sprintf(t,"MPH Announce Threshold = %d\r\n",settings.mph); DBG(t);
   sprintf(t,"Brake Tone = %d %d %d %d %d\r\n",settings.brake_tone_psi_low,settings.brake_tone_psi_high,settings.brake_tone_hz,settings.brake_tone_duration,settings.brake_tone_enable); DBG(t);
   sprintf(t,"Shift Tone = %d %d\r\n",settings.shift_tone_hz,settings.shift_tone_duration); DBG(t);
   sprintf(t,"Tire PSI = %d %d\r\n",settings.tire_low_psi[0],settings.tire_high_psi[0]); DBG(t);
   DBG("--- Enable Settings ---\r\n");
   sprintf(t,"Overrev Announce = %d\r\n",settings.overrev_announce); DBG(t);
   sprintf(t,"UpShift Announce = %d\r\n",settings.upshift_announce); DBG(t);
   sprintf(t,"DownShift Announce = %d\r\n",settings.downshift_announce); DBG(t);
   sprintf(t,"Oil Announce = %d\r\n",settings.oil_announce); DBG(t);
   sprintf(t,"Temperature Announce = %d\r\n",settings.temp_announce); DBG(t);
   sprintf(t,"Voltage Announce = %d\r\n",settings.volts_announce); DBG(t);
   sprintf(t,"LatGForce Announce = %d\r\n",settings.lateral_gforce_announce); DBG(t);
   sprintf(t,"LinGForce Announce = %d\r\n",settings.linear_gforce_announce); DBG(t);
   sprintf(t,"MPH Announce = %d\r\n",settings.mph_announce); DBG(t);
   sprintf(t,"WheelLock Announce = %d\r\n",settings.wheel_speed_enable); DBG(t);
   sprintf(t,"Lap Announce = %d\r\n",settings.lap_announce); DBG(t);
   DBG("-- Segment Settings ---\r\n");
   sprintf(t,"CHECKER %f %f\r\n",settings.checker_lattitude,settings.checker_longitude);
   DBG(t);
   for (i=0;i<MAX_SEGMENTS;i++)
   {
       memset(tt,0,sizeof(tt));
       sprintf(t,"SEGMENT %d START = %f %f STOP = %f %f %d\r\n",(i+1),settings.segment_start_lat[i],settings.segment_start_lng[i],settings.segment_stop_lat[i],settings.segment_stop_lng[i],(unsigned char)settings.segment_enable[i]);
       DBG(t);
   }
   DBG("-- Split Settings ---\r\n");
   for (i=0;i<MAX_SEGMENTS;i++)
   {
       memset(tt,0,sizeof(tt));
       if (settings.split_enable[i]==1) sprintf(tt,"ACTIVE\0"); else sprintf(tt,"DISABLED\0");
       sprintf(t,"SPLIT %d %f %f %s\r\n",(i+1),settings.split_lat[i],settings.split_lng[i],tt);
       DBG(t);
   }
   DBG("-- Phrase Settings ---\r\n");
   for (i=0;i<MAX_PHRASE;i++)
   {
       sprintf(t,"PHRASE %d %s %d\r\n",(i+1),&settings.phrase[i][0],settings.phrase_control[i]);
       DBG(t);
   }
   if (global.board_version==RACEVOICE_STANDALONE)
   {
        DBG("--- Sensor Values --\r\n");
   }
   else
   {
        DBG("--- Data System Values --\r\n");
   }
        sprintf(t,"GPS(Lat,Lng,Sats) = %f %f %d\r\n",global.gps_lat,global.gps_lng,global.gps_sats); DBG(t);
        sprintf(t,"Accel(x,y,z) = %f %f %f\r\n",global.linear_g,global.lateral_g,global.zaxis_g); DBG(t);
   if (global.board_version==RACEVOICE_STANDALONE)
   {
       sprintf(t,"TimeDate = %s\r\n",global.gps_zda); DBG(t);
   }
        
        
   
   if (global.board_version==RACEVOICE_STANDALONE)
   {
       DBG("--- Canbus Config Settings --\r\n");
       for (i=0;i<NUMBER_OF_CAN_IDS;i++)
       {
           sprintf(t,"CANLISTEN %d = 0x%08lx\r\n",i,global.CAN_IDS[i]);
           DBG(t);
       }
       for (i=0;i<NUMBER_OF_CAN_IDS;i++)
       {
           sprintf(t,"CANCONFIG %d = 0x%08lx ",i,settings.canconfig[i].can_id); DBG(t);
           sprintf(t,"Resource = 0x%02x ",settings.canconfig[i].resource); DBG(t);
           sprintf(t,"Type = %d ",settings.canconfig[i].type); DBG(t);
           sprintf(t,"Offset = %d ",settings.canconfig[i].offset); DBG(t);
           sprintf(t,"Mult = %f ",settings.canconfig[i].mult); DBG(t);
           for (j=0;j<8;j++)
           {
               if (j==0)
               {
                   DBG("Mask = ");
               }
               sprintf(t,"0x%02x \0",settings.canconfig[i].mask[j]);
               DBG(t);
           }
           DBG("\r\n");
       }
       
       
   }
   DBG("--- Admin Settings ---\r\n");
   sprintf(t,"Baud Rate = %d %d\r\n",settings.baud_rate,settings.can_listen); DBG(t);
   sprintf(t,"Voice Volume = %d\r\n",settings.voice_volume); DBG(t);
   sprintf(t,"Voice Speed = %d\r\n",settings.voice_speed); DBG(t);
   sprintf(t,"Voice Type = %d\r\n",settings.voice_type); DBG(t);
   sprintf(t,"Voice Pitch = %d\r\n",settings.voice_pitch); DBG(t);
   sprintf(t,"Track Index = %d %s\r\n",settings.trackindex,settings.trackname); DBG(t);
   sprintf(t,"Minimum Speed Threshold = %d\r\n",settings.minmph_threshold); DBG(t);
   sprintf(t,"RPM SHIFT Announcement Repeat = %d\r\n",settings.rpm_notice); DBG(t);
   sprintf(t,"TPS High Threshold = %d\r\n",settings.tps_high); DBG(t);
   sprintf(t,"TPS Low Threshold = %d\r\n",settings.tps_low); DBG(t);
   sprintf(t,"GPS Window = %d\r\n",settings.gpswindow); DBG(t);
   sprintf(t,"Record Mode = %d\r\n",global.flash_record); DBG(t);
   sprintf(t,"Flash Type = 0x%02x 0x%02x 0x%02x\r\n",global.flash_id[0],global.flash_id[1],global.flash_id[2]); DBG(t);
   
   if (global.flash_id[0]!=0)
   {
   inuse=(int)  (     ((double)global.flash_wr_addr/(double)global.flash_max_addr)  *(double)100);
   }
   else inuse=0;
   
   sprintf(t,"Flash RD=0x%08lx WR=0x%08lx MAX=0x%08lx INUSE=%d\r\n",global.flash_rd_addr,global.flash_wr_addr,global.flash_max_addr,inuse); DBG(t);
   //sprintf(t,"Suppress CLI Trace Echo = %d\r\n",settings.trace_level); DBG(t);
   //sprintf(t,"Coach Mode = %d\r\n",settings.coach_mode); DBG(t);
   switch(settings.dash_type)
   {
       case STANDALONE: sprintf(t,"Dash Type = STANDALONE %d\r\n",settings.dash_variant); break;
       case MOTEC: sprintf(t,"Dash Type = MOTEC %d\r\n",settings.dash_variant); break;
       case AIM: sprintf(t,"Dash Type = AIM %d\r\n",settings.dash_variant); break;
       case SMARTY: sprintf(t,"Dash Type = SMARTY %d\r\n",settings.dash_variant); break;
       case VBOX: sprintf(t,"Dash Type = VBOX %d\r\n",settings.dash_variant); break;
       case AUTOSPORTS: sprintf(t,"Dash Type = AUTOSPORT LABS %d\r\n",settings.dash_variant); break;
       case OBD_II: sprintf(t,"Dash Type = OBD_II %d\r\n",settings.dash_variant); break;
       case CUSTOM_CAN: sprintf(t,"Dash Type = CUSTOM %s\r\n",settings.canconfig_name); break;
       default: sprintf(t,"Dash Type = ????? %d\r\n",settings.dash_type); break;
   }
   DBG(t);
   DBG("--DONE--\r\n");
 
   for (i=0;i<5;i++)   DBG("\r\n");
    
    
}
void ShowMenu(void)
{
    int i=0;
    char t[128];
    for (i=0;i<5;i++)   DBG("\r\n");
    DBG("----- Shell Menu\r\n");
    DBG("SET RPM OVERREV THRESHOLD XXXX             : Setup the trigger level for overrev trigger\r\n");
    DBG("SET RPM HIGH THRESHOLD XXXX                : Setup the trigger level for up-shift trigger\r\n");
    DBG("SET RPM LOW THRESHOLD XXXX                 : Setup the trigger level for down-shift trigger\r\n");
    DBG("SET RPM OIL THRESHOLD XXXX                 : Setup the rpm level at which oil is checked\r\n");
    DBG("SET OIL THRESHOLD XXXX                     : Setup the trigger level for low oil trigger\r\n");
    DBG("SET TEMPERATURE THRESHOLD XXX              : Setup the trigger level for high temperature trigger\r\n");
    DBG("SET VOLTAGE THRESHOLD X.X                  : Setup the trigger level for low voltage trigger\r\n");
    DBG("SET LATERAL THRESHOLD X.X                  : Setup the trigger level for corner force\r\n");
    DBG("SET LINEAR THRESHOLD X.X                   : Setup the trigger level for braking force\r\n");
    DBG("SET MPH THRESHOLD XXXX                     : Setup the trigger level for mph reporting\r\n");
    DBG("SET BRAKE TONE A B C D E                   : Setup tone for brake (psi-low,psi-high, hz, duration, enable(0/1)\r\n");
    DBG("SET WHEELLOCKSPEED THRESHOLD XXXX          : Setup the triger pct level for wheel lock\r\n");
    DBG("SET WHEELLOCKBRAKE THRESHOLD XXXX          : Setup the triger pct level for wheel lock\r\n");
    DBG("SET ANNOUNCE OVERREV                       : (0/1)\r\n");
    DBG("SET SHIFT TONE A B                         : Setup shift tone hz and duration\r\n");
    DBG("SET ANNOUNCE UPSHIFT                       : (0/1/2/3)  on/off/tone\r\n");
    DBG("SET ANNOUNCE DOWNSHIFT                     : (0/1/2/3)  on/off/tone\r\n");
    DBG("SET ANNOUNCE OIL                           : (0/1)\r\n");
    DBG("SET ANNOUNCE TEMPERATURE                   : (0/1)\r\n");
    DBG("SET ANNOUNCE VOLTAGE                       : (0/1)\r\n");
    DBG("SET ANNOUNCE LatGFORCE                     : (0/1)\r\n");
    DBG("SET ANNOUNCE LinGFORCE                     : (0/1)\r\n");
    DBG("SET ANNOUNCE MPH                           : (0/1)\r\n");
    DBG("SET ANNOUNCE WHEELLOCK                     : (0/1)\r\n");
    DBG("SET CHECKER LAT LNG                        : Lap start point by GPS Pairs\r\n");
    DBG("SET SEGMENT XXX START LAT LNG STOP LAT LNG : Segment configuration by GPS Pairs\r\n");
    DBG("SET SEGMENT XXX (ENABLE/DISABLE)           : Control segment activation\r\n");
    DBG("SET SPLIT XXX LAT LNG                      : Split time configuration by GPS Pairs\r\n");
    DBG("SET SPLIT XXX (ENABLE/DISABLE)             : Split time activation\r\n");
    DBG("SET PHRASE XX YYY                          : Set speech phrase slot X with text Y\r\n");
    DBG("SET TIRE PSI XX YY                         : Set target tire psi XX=low YY=high\r\n");
    DBG("----- Admin Menu\r\n");
    DBG("SET BAUD RATE XXXX                 : Set canbus baud rate 125,250,500,1000\r\n");
    DBG("SET GPS WINDOW XXXX                : Set gps window size value (0,1,2,3,4)\r\n");
    DBG("DATA TRACE                         : Single Line Debug Trace for key values\r\n");
    DBG("CAN MSG VALUE                      : MSG (MPH/TPS/RPM/PSI/LAT/LAP/DIF) AND NUMERICAL VALUE\r\n");
    DBG("GPS TRACE (ON/OFF)                 : Debug trace for GPS values ON,OFF\r\n");
    DBG("CAN TRACE (ON/OFF)                 : Debug trace ON,OFF\r\n");
    DBG("ACCEL TRACE (ON/OFF)               : Debug trace ON,OFF\r\n");
    DBG("SET TPS LOW THRESHOLD XXXX         : Setup the trigger level for part throttle/coast detection\r\n");
    DBG("SET TPS HIGH THRESHOLD XXXX        : Setup the trigger level for full throttle detection\r\n");
    DBG("SET VERBOSE XXXX                   : Debug verbosity level (0=off)\r\n");
    DBG("SET VOICE TYPE XXX                 : Set the type (0-10)\r\n");
    DBG("SET VOICE PITCH XXX                : Set the voice pitch (1-99)\r\n");
    DBG("SET VOICE SPEED XXX                : Set the voice speed (1-10)\r\n");
    DBG("SET VOICE VOLUME XX                : Set the voice volume (0-9)\r\n");
    DBG("SET ANNOUNCE DIFF XXX              : Announce lap time PLUS/MINUS (0/1)\r\n");
    DBG("SET ANNOUNCE COAST XXX             : Announce throttle coasting (0/1)\r\n");
    DBG("SET ANNOUNCE SHIFT XXX             : Announce up or down shift (0/1)\r\n");
    DBG("SET TRACK INDEX XXX,YYY            : UI Index for track selection,name\r\n");
    DBG("SPEAK XXXXXX                       : Speak a phrase with upto 8 words\r\n");
    DBG("SHOW LIVE                          : Show all received data values\r\n");
    DBG("SHOW SETTINGS                      : Show all global set points\r\n");
    DBG("READ SETTINGS                      : Read and display settings from flash vector\r\n");
    DBG("SAVE SETTINGS                      : Force save and read/display from flash vector\r\n");
    DBG("SET DASH xxxxx                     : Configure the dashboard type as MOTEC or AIM\r\n");
    DBG("SET NAME xxxxx                     : Configure a unique name for this unit\r\n");
    DBG("FLASH CHECK                        : Check & report flash memory, clears all pointers\r\n");
    DBG("FLASH STATUS                       : See the current status pointers\r\n");
    DBG("FLASH RESET                        : Goto start of flash for readout\r\n");
    DBG("FLASH LAST                         : Goto next sector after the last record\r\n");
    DBG("FLASH ERASE                        : Erase the entire flash memory\r\n");
    DBG("FLASH READ                         : Readout the entire flash memory\r\n");
    DBG("FLASH RECORD ON/OFF/ENABLE         : Force Datalog ON/OFF or Enabled\r\n");
    DBG("FWERASE                            : Erase download sectors\r\n");
    DBG("FWUPDATE                           : Start firmware download process\r\n");
    DBG("FWCOMMIT                           : Commit to runtime sectors\r\n");
    DBG("VERSION                            : Show and speak the current version (use VERSION TALK)\r\n");
    DBG("---- Status\r\n");
    sprintf(t,"CanRX=%d CanTX=%d HB=%ld GPS=%ld,%ld BTLink=%d Record=%d\r\n",global.canrx,global.cantx,global.hb,global.gps_ticks,global.gps_rx_errors,global.btlink,global.flash_record); DBG(t);
    
    for (i=0;i<5;i++)   DBG("\r\n");
}

int GetDouble(char* ascii,double *v)
{
    *v=atof(ascii);
    return 2;
}
int GetChar(char* ascii,unsigned char* v)
{
    *v=atoi(ascii) & 0xff;
    return 2;
}
int GetInt(char* ascii,int* v)
{
    *v=atoi(ascii);
    return 2;
}

void DownloadFirmware(char* hex)
{
    static hex_record hr;
    int i;
    unsigned long fa=0;
    unsigned long fd=0;
    int valid=0;
    hex_read_record_ascii(&hr,hex);
    if (hex_verify(&hr)) valid=1;
    sprintf(otxt,"\r\nType=0x%02x Len=%d Address=0x%04x:%04x Valid=%d",hr.type,hr.len,hr.addrh,hr.addr,valid); DBG(otxt);
    if (hr.type==1 && hr.len==0)
    {
        DBG("END OF HEX\r\n");
        return;
    }
    if (hr.type==0 && valid==0)
    {
        DBG("HEX ERROR!\r\n");
        return;
    }
    // do we have a valid data payload type?
    if (valid && hr.type==0)
    {
        fa=hr.addrh;
        fa=fa<<16;
        fa|=hr.addr;
        

        // boot sector can be from 1FC0:0000 to 1FC0:2FEF
        if (fa>=BOOT_FLASH_BASE)
        {
            DBG(" --> Reserved Boot Section\0");
            
        }
        else if(fa>=RESERVED_FLASHSECTION)
        {
            DBG(" --> Reserved Flash Section\0");
        }
        else
        {
            fa+=FLASH_OFFSET;
            sprintf(otxt,"  -->Target=0x%08lx   ",fa); DBG(otxt);
            for (i=0;i<hr.len;i=i+4)
            {
                fd=hr.data[3+i]<<24 | hr.data[2+i]<<16 | hr.data[1+i]<<8 | hr.data[0+i];
                if(WordPGM(fa,fd)==0)
                {
                    DBG("FLASH ERROR!\r\n");
                    while(1) asm("nop");

                }
                fa+=4; // next 32-bit word address
            }
        }
    }
    DBG("\r\n");
    // write to flash
//bool hex_verify(hex_record* r
}


void hexDump (unsigned long addr, int len) {
    int i;
    int j=0;
    int first=1;
    char t1[32];
    unsigned long dv=0;
    
    
    for (i=0;i<len;i++)
    {
        if (j>=8 || first)
        {
            first=0;
            j=0;
            DBG("\r\n");
            sprintf(t1,"0x%08lx : \0",addr);
            DBG(t1);
        }

        
        dv=*(volatile unsigned long*)addr;
        sprintf(t1,"%08lx \0",dv); DBG(t1);
        j++;
        addr+=4;
        
    }
}


//** format is slot 0,1,2,3,4,5,6,7 etc

int ProgramCan(char* slot, char* id, char* mask, char* resource, char* type, char* offset, char* mult)
{
    int islot=atoi(slot);
    unsigned int icanid=0;
    unsigned int imask[10];
    int iresource=0;
    int ioffset=0;
    float imult=0;
    int itype=0;
    int i;
    char hv[5];
    if (islot>=NUMBER_OF_CAN_IDS)
    {
        sprintf(txttmp,"Error:Incorrect CanSlot=%d",islot);
        DBG(txttmp);
        return 0;
    }
    
    sprintf(txttmp,"\r\nCAN Slot =%d\r\n",islot); DBG(txttmp);
    memset(hv,0,sizeof(hv));
    sscanf(id,"%X",&icanid);
    sprintf(txttmp,"Got CanID=0x%04x\r\n",icanid); DBG(txttmp);
    
    for (i=0;i<8;i++)
    {
        memset(hv,0,sizeof(hv));
        hv[0]=*(mask+(i<<1));
        hv[1]=*(mask+(i<<1)+1);
        sscanf(hv,"%X",&imask[i]);
    }
    
    for (i=0;i<8;i++)
    {
        sprintf(txttmp,"Got Mask[%d]=0x%02x\r\n",i,imask[i]); DBG(txttmp);
    }
    
    sscanf(resource,"%X",&iresource);
    
    itype=atoi(type);
    ioffset=atoi(offset);
    imult=atof(mult);
    sprintf(txttmp,"Resource=0x%02x Type=%d Offset=%d Mult=%f\r\n",iresource,itype,ioffset,imult); DBG(txttmp);    
    
    settings.canconfig[islot].can_id=icanid;
    settings.canconfig[islot].resource=iresource;
    settings.canconfig[islot].type=itype;
    settings.canconfig[islot].offset=ioffset;
    settings.canconfig[islot].mult=imult;
    for (i=0;i<8;i++)
    {
     settings.canconfig[islot].mask[i]=imask[i];   
    }
}
void ProcessShell(char* cmd)
{
   int i=0;
   int k=0;
   int j=0;
   int valid=0;
   char * pch;
   char ch=0;
   int segcount=0;
   int tempval=0;
   unsigned long tt=0;
   // keep these off the stack
   static char c1[32];
   static char c2[32];
   static char c3[32];
   static char c4[64]; // text field
   static char c5[64]; // text field 
   static char c6[32];
   static char c7[32];
   static char c8[32];
   static char c9[32];
   static char c10[32];
   static char tmp[255];
   
   
   // grab a leading line with a : and parse it as an intel hex file
   // then put it into the upper part of flash memory
   if (fw_update && *(cmd+0)==':')
   {
       // don't send the leading :
       DownloadFirmware((cmd+1));
       return;
   }
   memset(c1,0,sizeof(c1));
   memset(c2,0,sizeof(c2));
   memset(c3,0,sizeof(c3));
   memset(c4,0,sizeof(c4));
   memset(c5,0,sizeof(c5));
   memset(c6,0,sizeof(c6));
   memset(c7,0,sizeof(c7));
   memset(c8,0,sizeof(c8));
   memset(c9,0,sizeof(c9));
   memset(c10,0,sizeof(c10));
   strcpy(tmp,cmd); 

   if (strstr(cmd,"SPEAK")!=NULL)
   {
     memset(tmp,0,sizeof(tmp));
     j=strlen(cmd);
     k=0;
     for (i=6;i<j;i++)
     {
         if (k>=(sizeof(tmp)-5)) break;
         tmp[k]=*(cmd+i);     
         k++;
     }
#if(0)
     DBG("SPEAK=[");
     DBG(tmp);
     DBG("]\r\n");
#endif
     
   }
   
   i=k=0;
   pch = strtok (cmd," ");
   while (pch != NULL)
   {
       i++;
       if (i>=11) break;
       if (i==1) strcpy(c1,pch);
       if (i==2) strcpy(c2,pch);
       if (i==3) strcpy(c3,pch);
       if (i==4) strcpy(c4,pch);
       if (i==5) strcpy(c5,pch);
       if (i==6) strcpy(c6,pch);
       if (i==7) strcpy(c7,pch);
       if (i==8) strcpy(c8,pch);
       if (i==9) strcpy(c9,pch);
       if (i==10) strcpy(c10,pch);
       pch = strtok (NULL, " ");
    }    
   
#if(0)
   DBG("TRACE1:["); DBG(c1); DBG("]"); DBG("\r\n");
   DBG("TRACE2:["); DBG(c2); DBG("]"); DBG("\r\n");
   DBG("TRACE3:["); DBG(c3); DBG("]"); DBG("\r\n");
   DBG("TRACE4:["); DBG(c4); DBG("]"); DBG("\r\n");
#endif   

    if (strstr(c1,"FWCOMMIT")!=NULL)
   {
       //if (fw_update)
       {
        DBG("--->STARTING UPDATE:");
        UpdateRunTimeFlash();   
       }
       valid=1;
       goto cmd_done;
   }
   if (strstr(c1,"FWUPDATE")!=NULL)
   {
       fw_update=1;
       valid=1;
       goto cmd_done;
       
   }
   if (strstr(c1,"FWERASE")!=NULL)
   { 
       DBG("\r\nWorking-->");
       valid=EraseFlashSegment(DSECTOR);
       if (valid==0)
       {
           DBG("Erase Fail\r\n");
       }
       else
       {
           DBG("Erase Pass\r\n");
           
       }
       valid=1;
       goto cmd_done;
   }
   
   
   if (strstr(c1,"PLAY")!=NULL)
   {
       if (strstr(c2,"MPH")!=NULL)
           PlayAudioInteger(atoi(c3),1);
       else
           PlayAudio(atoi(c2),1);
       goto cmd_done;
   }
   if (strstr(c1,"ERASE")!=NULL)
   {
       EraseAudio();
       goto cmd_done;
   }
   if (strstr(c1,"INIT")!=NULL)
   {
       InitAudio();
       goto cmd_done;
   }
   if (strstr(c1,"LINK")!=NULL)
   {
       direct_rc_link=1;
       goto cmd_done;
   }
   
  
   
   if (strstr(c1,"DUMP\0")!=NULL)
   {
       tt=atol(c2);
       if (tt==0)
       {
           hexDump(FLASH_BASE_RD,1024);
       }
       if (tt==1)
       {
           hexDump(FLASH_BASE_RD+FLASH_OFFSET,1024);
       }
       valid=1;
   }
   if (strstr(c1,"VERSION\0")!=NULL)
   {
       if (strstr(c2,"TALK\0")!=NULL) SpeakVersion(2); else SpeakVersion(0);
       valid=1;
       goto cmd_done;
   }
   
   if (strstr(c1,"GPS\0")!=NULL)
   {
       if (strstr(c2,"TRACE\0")!=NULL)
       {
           if (strstr(c3,"ON\0")!=NULL)
           {
               global.gps_trace=1;
           }
           else
           {
               global.gps_trace=0;
           }
           valid=1;
           goto cmd_done;
               
              
       }
   }
   
   if (strstr(c1,"ACCEL\0")!=NULL)
   {
       if (strstr(c2,"TRACE\0")!=NULL)
       {
           if (strstr(c3,"ON\0")!=NULL)
           {
               global.accel_trace=1;
           }
           else
           {
               global.accel_trace=0;
           }
           valid=1;
           goto cmd_done;
               
              
       }
   }
   
   if (strstr(c1,"DATA\0")!=NULL)
   {
       if (strstr(c2,"TRACE\0")!=NULL)
       {
           memset(tmp,0,sizeof(tmp));
#if(0)
           global.tps=10;
           global.rpm=750;
           global.engine_temp_f=175;
           global.oil_pressure_psi=38;
           global.brake_front_psi=0;
           global.volts=14.35;
           global.gps_lat=42.341028;
           global.gps_lng= -76.928861;
#endif
           sprintf(tmp,"*SOM*,%d,%d,%d,%d,%d,%0.1f,%f,%f,%f,%f,*EOM*\r\n",global.tps,global.rpm,global.engine_temp_f,global.oil_pressure_psi,global.brake_front_psi,global.volts,global.gps_lat,global.gps_lng,global.lateral_g,global.linear_g);
           DBG(tmp);
           valid=1;
           goto cmd_done;
       }
   }
   
   
   if (strstr(c1,"CAN\0")!=NULL)
   {
       
           if (strstr(c2,"TRACE")!=NULL)
           {
               if (strstr(c3,"ON")!=NULL) 
               {
                   global.can_trace=1; 
                   if (strstr(c4,"RAW")!=NULL) global.can_trace=2;
               }
               else global.can_trace=0;
               goto cmd_done;
           }
           
           if (strstr(c2,"CONFIG")!=NULL)
           {
               ProgramCan(c3,c4,c5,c6,c7,c8,c9);
               valid=2;
           }
       goto cmd_done;
           
   }
   
   
   if (strstr(c1,"FLASH\0")!=NULL)
   {
       if (strstr(c2,"STATUS\0")!=NULL)
       {
           FlashStatus();
           valid=1;
           goto cmd_done;
       }
       if (!ValidSpiFlash())
       {
           DBG("\r\nFLASH:NOT FOUND\r\n");
           valid=1;
           goto cmd_done;
       }

       if (strstr(c2,"READ\0")!=NULL)
       {
           if (c3[0]==0) global.read_from_flash=1;
           else
           {
            if (strstr(c3,"TRACE\0")!=NULL) global.read_from_flash=2;
            if (strstr(c3,"STEP\0")!=NULL) global.read_from_flash_step=1;
           }
           valid=1;
       }
       
       if (global.flash_erase)
       {
           DBG("\r\nFLASH:ERASE BUSY\r\n");
           valid=1;
           goto cmd_done;
       }
       if (strstr(c2,"RESET\0")!=NULL)
       {
                global.flash_rd_addr=0;
                valid=1;
       }
       if (strstr(c2,"LAST\0")!=NULL)
       {
                global.flash_last=1;
                valid=1;
       }
       
       if (strstr(c2,"ERASE\0")!=NULL)
       {
           global.flash_erase=1;
           valid=1;
       }
       
        
       if (strstr(c2,"PLAY\0")!=NULL)
       {
           global.flash_play=1;
           valid=1;
       }
       
       if (strstr(c2,"CHECK\0")!=NULL)
       {
           global.flash_check=1;
           valid=1;
       }
       
       if (strstr(c2,"REC\0")!=NULL)
       {
           if (strstr(c3,"ENABLE")!=NULL)
           {
               settings.log_enabled=1;
               valid=2; 
               goto cmd_done;
           }
           
           if (strstr(c3,"ON")!=NULL)
           {
                if (strstr(c4,"TRACE")!=NULL) global.flash_record=101;
                   else
                    global.flash_record=100; 
           }
           else global.flash_record=0;
       
           valid=1;
       }
       goto cmd_done;
   }
   
   if (strstr(c1,"SPEAK\0")!=NULL)
   {
       Speak(tmp,3);
       valid=1;
       goto cmd_done;
   }

   if ((strstr(c1,"SHOW\0")!=NULL) && (strstr(c2,"LIVE\0")!=NULL))
   {
       valid=1;
       ShowLiveValues();
       goto cmd_done;
   }
   if ((strstr(c1,"SHOW\0")!=NULL) && (strstr(c2,"DASH\0")!=NULL))
   {
       valid=1;
       global.dash_stream=1;
       goto cmd_done;
   }


   if (strstr(c2,"SETTINGS\0")!=NULL)
   {
       if (strstr(c1,"SHOW\0")!=NULL) {valid=1; ShowSettings();}
       if (strstr(c1,"READ\0")!=NULL) {valid=1; ReadSettingsFromFlash(1);}
       if (strstr(c1,"SAVE\0")!=NULL) {valid=1; SaveSettingsToFlash();}
       goto cmd_done;
   }
   if (strstr(c1,"SET\0"))
   {
       
       if (strstr(c2,"BAUD")!=NULL)
       {
           if (strstr(c3,"RATE")!=NULL)
           {
               GetInt(c4,&tempval);
               if (tempval==_125K_ || tempval==_250K_ || tempval==_500K_ || tempval==_1000K_)
               {
                   settings.baud_rate=tempval;
                   SetBaud(settings.baud_rate);
                   valid=2;
               }
               
               if (strstr(c5,"LISTEN"))
               {
                   settings.can_listen=1;
               }
               else
               {
                  settings.can_listen=0;   
               }
               goto cmd_done;
           }
       }
       
       if (strstr(c2,"SHIFT")!=NULL)
       {
           if (strstr(c3,"TONE")!=NULL)
           {
               valid=GetInt(c4,&settings.shift_tone_hz);
               valid=GetInt(c5,&settings.shift_tone_duration);
               valid=2;
               goto cmd_done;
               
           }
       }
       if (strstr(c2,"BRAKE")!=NULL)
       {
           if (strstr(c3,"TONE")!=NULL)
           {
               valid=GetInt(c4,&settings.brake_tone_psi_low);
               valid=GetInt(c5,&settings.brake_tone_psi_high);
               valid=GetInt(c6,&settings.brake_tone_hz);
               valid=GetInt(c7,&settings.brake_tone_duration);
               valid=GetInt(c8,&settings.brake_tone_enable);
               valid=2;
               goto cmd_done;
           }
       }
       if (strstr(c2,"GPS")!=NULL)
       {
           if (strstr(c3,"WINDOW")!=NULL)
           {
             valid=GetInt(c4,&settings.gpswindow);
             goto cmd_done;

           }
           
           
       }
       
       if (strstr(c2,"TIRE")!=NULL && strstr(c3,"PSI")!=NULL)
       {
           settings.tire_low_psi[0]=atoi(c4);
           settings.tire_high_psi[0]=atoi(c5);
           goto cmd_done;
       }
       
       if (strstr(c2,"PHRASE")!=NULL)
       {
           segcount=atoi(c3);
           segcount=segcount-1;
           if (segcount>=0 && segcount<=MAX_PHRASE)
           {
               // erase current phrase
               for (k=0;k<PHRASE_LEN;k++)
               {
                   ch=c4[k];
                   settings.phrase[segcount][k]=ch;
               }
               
               settings.phrase_control[segcount]=0; 
               i=atoi(c5);
               if (i>=0 && i<=20) settings.phrase_control[segcount]=i;
               valid=2;
               goto cmd_done;
           }
       }
       if (strstr(c2,"SPLIT")!=NULL)
       {
           segcount=atoi(c3);
           segcount=segcount-1; // make it zero based
           if (strstr(c4,"ENABLE")!=NULL)
           {
             if (strstr(c5,"1")!=NULL) settings.split_enable[segcount]=1; else settings.split_enable[segcount]=0;   
             valid=2; // force it to be accepted
           }
           else
           {
               if (segcount>=0 && segcount<=MAX_SEGMENTS)
               {
                valid=GetDouble(c5,&settings.split_lat[segcount]);
                valid=GetDouble(c6,&settings.split_lng[segcount]);
                }
           }
       goto cmd_done;
       }
       
       if (strstr(c2,"CHECKER")!=NULL)
       {
           valid=GetDouble(c3,&settings.checker_lattitude);
           valid=GetDouble(c4,&settings.checker_longitude);
           goto cmd_done;
 
           
       }
       if (strstr(c2,"SEGMENT")!=NULL)
       {
           segcount=atoi(c3);
           segcount=segcount-1; // make it zero based
           if (strstr(c4,"ENABLE")!=NULL)
           {
               settings.segment_enable[segcount]=atoi(c5);
               valid=2; // force it to be accepted
           }
           
           if (strstr(c4,"START")!=NULL)
           {
            if (segcount>=0 && segcount<=MAX_SEGMENTS)
            {
               valid=GetDouble(c5,&settings.segment_start_lat[segcount]);
               valid=GetDouble(c6,&settings.segment_start_lng[segcount]);
               valid=GetDouble(c8,&settings.segment_stop_lat[segcount]);
               valid=GetDouble(c9,&settings.segment_stop_lng[segcount]);
            }
           }
       goto cmd_done;
       
       }
       if (strstr(c2,"TRACK\0")!=NULL)
       {
           if (strstr(c3,"INDEX\0")!=NULL)
           {
                valid=GetInt(c4,&settings.trackindex);
                memset(settings.trackname,0,sizeof(settings.trackname));
                if (strlen(c5)>0)
                {
                    strncpy(settings.trackname,c5,sizeof(settings.trackname));
                }
           }
              goto cmd_done;

       }
       
       if (strstr(c2,"NAME\0")!=NULL)
       {
           if (strlen(c3)>0 && strlen(c3)<UNIT_NAME_SIZE)
           {
            memset(settings.unit_name,0,sizeof(settings.unit_name));
            strcpy(settings.unit_name,c3);
            valid=2;
           }
           goto cmd_done;
       }
       if (strstr(c2,"DASH\0")!=NULL)
       {
           DISABLE_IRQ();
           SetBaud(0);
           settings.tps_high=TPS_HIGH_DEFAULT;
           if (strstr(c4,"1\0")!=NULL && valid==2) settings.dash_variant=1; else settings.dash_variant=0;
           if (strstr(c3,"AUTOSPORT\0")!=NULL) {valid=2; settings.dash_type=AUTOSPORTS; }
           if (strstr(c3,"MOTEC\0")!=NULL) {valid=2; settings.dash_type=MOTEC;}
           if (strstr(c3,"AIM\0")!=NULL) { valid=2; settings.dash_type=AIM; }
           if (strstr(c3,"SMARTY\0")!=NULL) { valid=2; settings.dash_type=SMARTY; settings.baud_rate=_1000K_;  }
           if (strstr(c3,"VBOX\0")!=NULL) { valid=2; settings.dash_type=VBOX; }
           if (strstr(c3,"OBDII\0")!=NULL) { valid=2; settings.dash_type=OBD_II; settings.tps_high=TPS_OBD_HIGH_DEFAULT;}
           if (strstr(c3,"STANDALONE\0")!=NULL) { valid=2; settings.dash_type=STANDALONE;}
           if (strstr(c3,"CUSTOM\0")!=NULL) 
           { 
               memset(settings.canconfig_name,0,PHRASE_LEN);
               sprintf(settings.canconfig_name,"%s\0",c4);
               i=atoi(c5);
               if (i!=0) settings.baud_rate=i;
               settings.dash_type=CUSTOM_CAN; 
               valid=2; 
           }
           
           if (valid==2)
           {
               SetBaud(settings.baud_rate);
           }
           ENABLE_IRQ();
            goto cmd_done;
       }
       if (strstr(c2,"VOICE\0")!=NULL && strstr(c3,"SPEED\0")!=NULL)
       {
            valid=GetInt(c4,&settings.voice_speed);
            if (valid)
            {
               SpeakControl(VOICESPEED,settings.voice_speed);
            }
               goto cmd_done;
      }

       if (strstr(c2,"VOICE\0")!=NULL && strstr(c3,"VOLUME\0")!=NULL)
       {
            valid=GetInt(c4,&settings.voice_volume);
            if (valid)
            {
               SpeakControl(VOLUME,settings.voice_volume);
            }
               goto cmd_done;
      }

       if (strstr(c2,"VOICE\0")!=NULL && strstr(c3,"TYPE\0")!=NULL)
       {
            valid=GetInt(c4,&settings.voice_type);
            if (valid)
            {
               SpeakControl(VOICE,settings.voice_type);
            }
               goto cmd_done;
      }
       
       if (strstr(c2,"VOICE\0")!=NULL && strstr(c3,"PITCH\0")!=NULL)
       {
            valid=GetInt(c4,&settings.voice_pitch);
            if (valid)
            {
               SpeakControl(PITCH,settings.voice_pitch);
            }
               goto cmd_done;
      }
       if (strstr(c2,"ANNOUNCE\0")!=NULL)
       {
           if (strstr(c3,"LAP\0")!=NULL)
           {
               valid=GetInt(c4,&settings.lap_announce);
               goto cmd_done;
           }
           
           if (strstr(c3,"WHEELLOCK\0")!=NULL)
           {
               valid=GetInt(c4,&settings.wheel_speed_enable);
                   goto cmd_done;
           }

           
           if (strstr(c3,"OVERREV\0")!=NULL)
           {
               valid=GetInt(c4,&settings.overrev_announce);
                   goto cmd_done;
           }
           if (strstr(c3,"UPSHIFT\0")!=NULL)
           {
               valid=GetInt(c4,&settings.upshift_announce);
               goto cmd_done;
           }
           if (strstr(c3,"DOWNSHIFT\0")!=NULL)
           {
               valid=GetInt(c4,&settings.downshift_announce);
               goto cmd_done;
           }
           if (strstr(c3,"OIL\0")!=NULL)
           {
                   valid=GetInt(c4,&settings.oil_announce);
               goto cmd_done;
           }
           if (strstr(c3,"TEMP\0")!=NULL)
           {
               valid=GetInt(c4,&settings.temp_announce);
               goto cmd_done;
           }
           if (strstr(c3,"VOLT\0")!=NULL)
           {
               valid=GetInt(c4,&settings.volts_announce);
               goto cmd_done;
           }
           if (strstr(c3,"LATGFORCE\0")!=NULL)
           {
               valid=GetInt(c4,&settings.lateral_gforce_announce);
               goto cmd_done;
           }
           if (strstr(c3,"LINGFORCE\0")!=NULL)
           {
               valid=GetInt(c4,&settings.linear_gforce_announce);
               goto cmd_done;
           }
           if (strstr(c3,"MPH\0")!=NULL)
           {
               valid=GetInt(c4,&settings.mph_announce);
               goto cmd_done;
           }
               goto cmd_done;
       }

       if (strstr(c2,"VERBOSE\0")!=NULL)
       {
            valid=GetChar(c3,&settings.trace_level);   
               goto cmd_done;
       }
       
       
        if (strstr(c2,"RPM\0")!=NULL && strstr(c3,"OVERREV\0")!=NULL) 
        {
           valid=GetInt(c5,&settings.rpm_overrev);
               goto cmd_done;
        }
        if (strstr(c2,"RPM\0")!=NULL && strstr(c3,"HIGH\0")!=NULL) 
        {
           valid=GetInt(c5,&settings.rpm_high);
               goto cmd_done;
        }
        if (strstr(c2,"RPM\0")!=NULL && strstr(c3,"LOW\0")!=NULL) 
        {
           valid=GetInt(c5,&settings.rpm_low);
           goto cmd_done;
        }
        if (strstr(c2,"RPM\0")!=NULL && strstr(c3,"OIL\0")!=NULL) 
        {
           valid=GetInt(c5,&settings.rpm_oil_threshold);
           goto cmd_done;
        }

        if (strstr(c2,"TPS\0")!=NULL && strstr(c3,"HIGH\0")!=NULL) 
        {
           valid=GetInt(c5,&settings.tps_high);
               goto cmd_done;
        }
        if (strstr(c2,"TPS\0")!=NULL && strstr(c3,"LOW\0")!=NULL) 
        {
           valid=GetInt(c5,&settings.tps_low);
               goto cmd_done;
        }

       if (strstr(c3,"THRESHOLD\0")!=NULL)
       {
           
           if (strstr(c2,"WHEELLOCKSPEED\0")!=NULL) valid=GetInt(c4,&settings.wheel_speed_delta);
           if (strstr(c2,"WHEELLOCKBRAKE\0")!=NULL) valid=GetInt(c4,&settings.wheel_speed_brake_threshold);
           if (strstr(c2,"OIL\0")!=NULL) valid=GetInt(c4,&settings.oil_low);
           if (strstr(c2,"TEMPERATURE\0")!=NULL) valid=GetInt(c4,&settings.temp_high_f);
           if (strstr(c2,"VOLT\0")!=NULL)  valid=GetDouble(c4,&settings.volts);
           if (strstr(c2,"MPH\0")!=NULL) valid=GetInt(c4,&settings.mph);
           if (strstr(c2,"LATERAL\0")!=NULL)  valid=GetDouble(c4,&settings.lateral_g_high_trigger);
           if (strstr(c2,"LINEAR\0")!=NULL)  valid=GetDouble(c4,&settings.linear_g_high_trigger);
               goto cmd_done;
       }
   }

   if (strstr(c1,"?")!=NULL) 
   {
        ShowMenu();   
        global.can_sim=0;
        valid=1;
   }
    
   if (!valid)
   {
       DBG("    >> Invalid Command\r\n");
   }
   
   cmd_done:
   if (valid==2) SaveSettingsToFlash();
//   if (valid>=10) can_sim=1; 
   //sprintf(tmp,"\r\nC1=%s C2=%s C3=%s C4=%s\r\n",c1,c2,c3,c4);
   //DBG(tmp);
   
}





void HandleCustomSpeechIRQ(unsigned char index,unsigned char data)
{
    // do not accept data if we have something in process already
    if (global.custom_speech_trigger) return;
    
    if (index==0xff)
    {
        if (global.custom_speech_trigger==0)
        {
            global.custom_speech_trigger=1;
        }
    }
    else
    {
        if (index<CUSTOM_SPEECH_LEN)
        {
            global.custom_speech[index]=data;
        }
    }
   
}

void HandleCustomSpeech(void)
{
    int i;
    int nulcount=0;
    if (global.custom_speech_trigger==1)
    {
        for(i=0;i<8;i++) 
        {
            if (global.custom_speech[i]==0) nulcount++;
        }
        for (i=0;i<16;i++)
        {
            // convert any nulls or illegal characters to spaces
            if (global.custom_speech[i]<32 || global.custom_speech[i]>'z') global.custom_speech[i]=' ';
        }
        global.custom_speech[16]=0; // make sure we have a null terminator
        DBG("-- WILL SPEAK --\r\n");
        for (i=0;i<16;i++)
        {
            sprintf(txttmp,"0x%02x - %c\r\n",global.custom_speech[i],global.custom_speech[i]);
            DBG(txttmp);
        }
        DBG("----\r\n");
        if (nulcount==8)
        {
            DBG("Error: First 8-bytes were NULL/Space\r\n");
        }
        else
        {
            sprintf(txttmp,"Custom Voice Speed=%d Pitch=%d Type=%d\r\n",global.can_voice_speed,global.can_voice_pitch,global.can_voice_type); DBG(txttmp);
            SpeakControl(VOICESPEED,(global.can_voice_speed&0x0f));  
            SpeakControl(VOICE,(global.can_voice_type&0xf)); 
            SpeakControl(PITCH,global.can_voice_pitch);
               Speak(global.custom_speech,3);
            SpeakControl(VOICESPEED,(settings.voice_speed&0x0f));  
            SpeakControl(VOICE,(settings.voice_type&0xf)); 
            SpeakControl(PITCH,settings.voice_pitch);
        }
        memset(global.custom_speech,0,sizeof(global.custom_speech));
        global.custom_speech_trigger=0;
    }
    
}

void HandleGPS(void)
{
    static int loops=0;
    if (global.gps_update)
    {
        loops++;
        if (loops>=10)
        {
            global.gps_distance=gps_distance(global.gps_lat,global.gps_lng);
            CalcPredictive();
            loops=0;
            
        }
        //HandleSegmentsIRQ();
        global.gps_update=0;
        if (global.gps_trace)
        {
            sprintf(txttmp,"GPS=[%f,%f] Mph=%d Sats=%d\r\n",global.gps_lat,global.gps_lng,global.mph,global.gps_sats);
            DBG(txttmp);
        }
    }  

    if (global.board_version!=RACEVOICE_STANDALONE) return;
    GPS_Init();
    

}

void FlushSerial(void)
{
    int i=0;
    int ch=1;
    while(ch>0)
    {
        ch=GetCh();
        i++;
        if (i>1000) break;
    }
}


void RotateLed(void)
{
    static int lstate=0;
    static int lc=0;
    
    return;
    
    lc++;
    if (lc>=256)
    {
        lc=0;
    }
    else return;
    
    switch(lstate)
    {
        case 0:
            BLUE_LEDOn(); GREEN_LEDOff(); YELLOW_LEDOff();
            break;
        case 1: 
            BLUE_LEDOff(); GREEN_LEDOn(); YELLOW_LEDOff();
            break;
        case 2:             
            BLUE_LEDOff(); GREEN_LEDOff(); YELLOW_LEDOn();
        break;
        default:lstate=0; break;
    }
    lstate++;
    if (lstate>=2) lstate=0;
}
void HandleSerial(void)
{
    int ch=0;
    static int bi=0;
    static char buf[255];
    static int parse=0;
    static int first=1;
    
    SerialTop:
 
           if (first)
           {
              // DBG("Clear Buffer!\r\n");
               memset(buf,0,sizeof(buf));
               first=0;
           }
    U1CTSOff();
    //SendDBGByte(0x01);
    ch=GetCh();
    //SendDBGByte(0x02);
    U1CTSOn();

    if (ch>0)
    {
        global.dash_stream=0;
        global.gps_trace=0;
        global.can_trace=0; // shutoff can trace at first character injection
        if (global.flash_record==2) global.flash_record=0; // shutoff any recording session as well
        //SendDBGByte(ch);
        // if we are in can sim ... then hold to parse
       if (global.can_sim) parse=1;
       if (!global.can_sim) 
       {
           if (ch!='\r' && ch!='\n')  SendDBGByte(ch);
       }
        
       if (ch!=13)
        {
           if (ch==8)
           {
               // backspace
               if (bi>0)
               {
                   bi--;
                   buf[bi]=0;
               }
               
           }
           else
           {
                ch=toupper(ch);
                 buf[bi]=ch;
                    bi++;
                    if (bi>=sizeof(buf))
                    {
                        bi=0;
                        memset(buf,0,sizeof(buf));
                    }
            }
        }
    }
    if (ch==13)
    {
        if (global.can_sim) BLUE_LEDOn();
        if (bi==0)
        {
            if (!global.can_sim) Prompt();
        }
        else
        {
         ProcessShell(buf);
         if (!global.can_sim) Prompt();
         bi=0;
         memset(buf,0,sizeof(buf));
        }
         if (global.can_sim) BLUE_LEDOff();
         
         parse=0;
        
    }
        
    if (parse) goto SerialTop;
}

void FlashCanLED(void)
{
    static unsigned long got_can_rx=0;
    got_can_rx++;
#ifndef DEBUG_BLUE_LED
    if (got_can_rx<2) BLUE_LEDOn(); else BLUE_LEDOff();
    if (got_can_rx>4) got_can_rx=0;
#endif
    
}

void TestFlash(void)
{
    unsigned long j=0;
    unsigned long jl=1000000;
    while(1)
    {
     for (j=0;j<jl;j++) BLUE_LEDOn();
     for (j=0;j<jl;j++) BLUE_LEDOff();

    
    }

}


void ResetAllCanFilters(void)
{
    C1FLTCON0bits.FLTEN0=0; while(C1FLTCON0bits.FLTEN0) Nop();
    C1FLTCON0bits.FLTEN1=0; while(C1FLTCON0bits.FLTEN1) Nop();
    C1FLTCON0bits.FLTEN2=0; while(C1FLTCON0bits.FLTEN2) Nop();
    C1FLTCON0bits.FLTEN3=0; while(C1FLTCON0bits.FLTEN3) Nop();
    C1FLTCON1bits.FLTEN4=0; while(C1FLTCON1bits.FLTEN4) Nop();
    C1FLTCON1bits.FLTEN5=0; while(C1FLTCON1bits.FLTEN5) Nop();


    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER0, 0x50, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER0);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER1, 0x51, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER1);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER2, 0x52, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER2);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER3, 0x53, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER3);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER4, 0x54, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER4);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER5, 0x55, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER5);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER6, 0x56, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER6);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER7, 0x57, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER7);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER8, 0x58, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER8);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER9, 0x59, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER9);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER10, 0x5a, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER10);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER11, 0x5b, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER11);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER12, 0x5c, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER12);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER13, 0x5d, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER13);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER14, 0x5e, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER14);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER15, 0x5f, CAN_SID);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER15);
 
    PLIB_CAN_ChannelForTransmitSet(CAN_ID_1, CAN_CHANNEL14, 1, CAN_TX_RTR_DISABLED, CAN_LOW_MEDIUM_PRIORITY);

}


void SetCanFilters(int dash)
{

    ResetAllCanFilters();


    if (dash==CUSTOM_CAN)
    {
        SetupCustomCan();
        return;
        
    }

    if (dash==OBD_II)
    {
        SetupOBD_IICan();
        return;
    }

    
    switch(dash)
    {
        case VBOX:
        C1FLTCON0bits.FLTEN0=0; while(C1FLTCON0bits.FLTEN0) Nop();
        C1FLTCON0bits.FLTEN1=0; while(C1FLTCON0bits.FLTEN1) Nop();
        C1FLTCON0bits.FLTEN2=0; while(C1FLTCON0bits.FLTEN2) Nop();
        C1FLTCON0bits.FLTEN3=0; while(C1FLTCON0bits.FLTEN3) Nop();

        C1RXF0bits.SID=VBOX_LAT;
        C1RXF1bits.SID=VBOX_LNG_VEL;
        C1RXF2bits.SID=VBOX_GFORCE;
        C1RXF3bits.SID=VBOX_LAP;

        C1FLTCON0bits.FLTEN0=1;
        C1FLTCON0bits.FLTEN1=1;
        C1FLTCON0bits.FLTEN2=1;
        C1FLTCON0bits.FLTEN3=1;
    
        break;
        
        case SMARTY:
        C1FLTCON0bits.FLTEN0=0; while(C1FLTCON0bits.FLTEN0) Nop();
        C1FLTCON0bits.FLTEN1=0; while(C1FLTCON0bits.FLTEN1) Nop();
        C1FLTCON0bits.FLTEN2=0; while(C1FLTCON0bits.FLTEN2) Nop();
        C1FLTCON0bits.FLTEN3=0; while(C1FLTCON0bits.FLTEN3) Nop();
        C1FLTCON1bits.FLTEN4=0; while(C1FLTCON1bits.FLTEN4) Nop();
        C1FLTCON1bits.FLTEN5=0; while(C1FLTCON1bits.FLTEN5) Nop();

        C1RXF0bits.SID=SMARTY_GPS;
        C1RXF1bits.SID=SMARTY_1056;
        C1RXF2bits.SID=SMARTY_1057;
        C1RXF3bits.SID=SMARTY_1058;
        C1RXF4bits.SID=SMARTY_1059;
        C1RXF5bits.SID=SMARTY_1060;

        C1FLTCON0bits.FLTEN0=1;
        C1FLTCON0bits.FLTEN1=1;
        C1FLTCON0bits.FLTEN2=1;
        C1FLTCON0bits.FLTEN3=1;
        C1FLTCON1bits.FLTEN4=1;
        C1FLTCON1bits.FLTEN5=1;
        
        break;
            
        default: 
        break;
    }
    
    // change filter 11-12 to listen for extended messages 0xA100 to 0xA107 from auto sport labs
    // a100
    // a101
    // a102
    // a103
    // a104
    // a105
    // a106
    // a107
    
    PLIB_CAN_FilterDisable(CAN_ID_1, CAN_FILTER11);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER11, 0x0000A100, CAN_EID);
    PLIB_CAN_FilterMaskConfigure(CAN_ID_1, CAN_FILTER_MASK2, 0x3FFFFFF1, CAN_EID, CAN_FILTER_MASK_IDE_TYPE);
    PLIB_CAN_FilterToChannelLink(CAN_ID_1, CAN_FILTER11, CAN_FILTER_MASK2,CAN_CHANNEL11);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER11);

     PLIB_CAN_FilterDisable(CAN_ID_1, CAN_FILTER12);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER12, 0x0000A101, CAN_EID);
    PLIB_CAN_FilterMaskConfigure(CAN_ID_1, CAN_FILTER_MASK3, 0x3FFFFFF1, CAN_EID, CAN_FILTER_MASK_IDE_TYPE);
    PLIB_CAN_FilterToChannelLink(CAN_ID_1, CAN_FILTER12, CAN_FILTER_MASK3,CAN_CHANNEL12);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER12);


    // change filter 13 to listen for extended messages on TPMS
    PLIB_CAN_FilterDisable(CAN_ID_1, CAN_FILTER13);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER13, 0x18fef433, CAN_EID);
    PLIB_CAN_FilterMaskConfigure(CAN_ID_1, CAN_FILTER_MASK1, 0x3FFFFFFF, CAN_EID, CAN_FILTER_MASK_IDE_TYPE);
    PLIB_CAN_FilterToChannelLink(CAN_ID_1, CAN_FILTER13, CAN_FILTER_MASK1,CAN_CHANNEL13);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER13);

#if(0)
    // change filter 13 to listen for extended messages on AIM GPS
    PLIB_CAN_FilterDisable(CAN_ID_1, CAN_FILTER13);
    PLIB_CAN_FilterConfigure(CAN_ID_1, CAN_FILTER13, 0x4003C07, CAN_EID);
    PLIB_CAN_FilterMaskConfigure(CAN_ID_1, CAN_FILTER_MASK1, 0x3F000000, CAN_EID, CAN_FILTER_MASK_IDE_TYPE);
    PLIB_CAN_FilterToChannelLink(CAN_ID_1, CAN_FILTER13, CAN_FILTER_MASK1,CAN_CHANNEL13);
    PLIB_CAN_FilterEnable(CAN_ID_1, CAN_FILTER13);
#endif
    
    //C1FLTCON3bits.FLTEN14=1;
}


void SetBaud(int rate)
{
#ifdef FORCE_BAUD
    settings.baud_rate=rate=FORCE_BAUD;
    
#endif

    CAN_SHDNOn(); // disable the CANBus driver...
    if (rate==0) return;
    
    PLIB_CAN_ModuleEventDisable(CAN_ID_1 , 0|CAN_RX_EVENT);
    PLIB_INT_SourceDisable(INT_ID_0,INT_SOURCE_CAN_1);
  
    C1CONbits.REQOP = 4;
    while(C1CONbits.OPMOD != 4) Nop();
    
    
    //sprintf(tv,"CFG1A=0x%08lx\r\n",C1CFG); DBG(tv);
    
    switch(settings.dash_type)
    {
        case SMARTY: rate=_1000K_; break;
        case VBOX: rate=_1000K_; break;
        default:break;
    }

    // 87.5% sampling point values, 80MHz clock, PIC32 mode
    //    http://www.bittiming.can-wiki.info/
    switch(rate)
    {
        case _125K_: C1CFG=0x1bc13; break;
        case _250K_: C1CFG=0x2bf07; break;
        case _500K_: C1CFG=0xc1bc04; break;
        case _1000K_: C1CFG=0x8404; break;
        default: break; // default is 250K
    }
//    C1CFG=0x2bf03; // 500kb
    //sprintf(tv,"CFG1B=0x%08lx\r\n",C1CFG); DBG(tv);

    SetCanFilters(settings.dash_type);
    C1CONbits.REQOP = 0;
    while(C1CONbits.OPMOD != 0) Nop();
   PLIB_CAN_ModuleEventEnable(CAN_ID_1 , 0|CAN_RX_EVENT);
   PLIB_INT_SourceEnable(INT_ID_0,INT_SOURCE_CAN_1);
   CAN_SHDNOff(); // turn on the driver
   
   

    
}

// main application for the voice processor
void VoiceAppInitMain(void)
{
    unsigned long i=0;
    
    memset(delta_string,0,sizeof(delta_string));
    memset(&global,0,sizeof(global));
     
   for (i=0;i<64;i++) DBG("\r\n\0");
   DBG("Starting...\r\n");
   GREEN_LEDOff();
   BLUE_LEDOff();
   InitSettings();
   SetBaud(settings.baud_rate);
   InitSpeech();
   DBG("Active...\r\n");
   FindBoardVersion();
   SpeakVersion(1);
   global.can_trace=0;
   for (i=0;i<LED_ON_RATE;i++) 
    {
        BLUE_LEDOn();
        GREEN_LEDOn(); 
    } 
    BLUE_LEDOff();
    GREEN_LEDOff();
    GREEN_LEDOn();
    FlushSerial();
    DRV_TMR_AlarmEnable(appData.handleTimer0,true);
    DRV_TMR_AlarmEnable(appData.handleTimer2,true);
    global.system_ready=_READYFLAG_;

}


void LinkRC(void)
{
    char txt[32];
    int ch1;
    int ch2;
    
    if (SCTSStateGet()==0)  U1CTSOff();
    else
    {
         U1CTSOn();
         return;
    }
    
    ch1=GetCh();
    U1CTSOn(); // stop any data from the PC
    if (ch1>=0) 
    {
        //sprintf(txt,"GOT=[%c,%d]\r\n",ch1,ch1); DBG(txt);
        // send a PC character to the RC
        //if (ch1=='x' || ch1=='X') ch1=0x01; // control+a
        SpeechByte(ch1);
    }
    ch2=GetChSp();
    if (ch2>=0)
    {
        // send a RC back to the PC
        SendDBGByte((unsigned char)ch2);
    }
    
}

int IsTimingUpdate(double last)
{  
    static char last_run[32];
    static char current_run[32];
    char c1,c2,c3;
    char l1,l2,l3;
    char ch;
    int i;
    int len=0;
    int idx=0;
    int update=1;
    
    sprintf(current_run,"%f",global.running_lap_time);
    len=strlen(current_run);
    for (i=0;i<len;i++)
    {
        ch=current_run[i];
        if (idx==1)  c1=current_run[i];
        if (idx==2)  c2=current_run[i];
        if (idx==3)  c3=current_run[i];
        if (ch=='.' || idx!=0) idx++;
    }

    idx=0;
    sprintf(last_run,"%f",last);
    len=strlen(last_run);
    for (i=0;i<len;i++)
    {
        ch=last_run[i];
        if (idx==1)  l1=last_run[i];
        if (idx==2)  l2=last_run[i];
        if (idx==3)  l3=last_run[i];
        if (ch=='.' || idx!=0) idx++;
    }
    
    //if (c1==l1) update=0;
    if (c2==l2) update=0;
    if (c3==l3) update=0;
    return update;
}
void HandleDashStream(void)
{
    char tmp[128];
    static double last_run=0;
    int i;
  
    static int loops=0;
    // about 5hz
    if (global.dash_stream==0)
    {
        global.dash_send=0;
        loops=0;
        return;
    }
    if (global.dash_send>=1)
    {
        if (last_run!=0)
        {
            for (i=0;i<20;i++)
            {
                if (IsTimingUpdate(last_run)) break;
            }
        }
        last_run=global.running_lap_time;
        
        sprintf(tmp,"%d,",global.mph); DBG(tmp);
        sprintf(tmp,"%d,",global.rpm); DBG(tmp);
        sprintf(tmp,"%d,",global.oil_pressure_psi); DBG(tmp);
        sprintf(tmp,"%d,",global.engine_temp_f); DBG(tmp);
        sprintf(tmp,"%d,",global.tps); DBG(tmp);
        sprintf(tmp,"%d,",global.lapnumber); DBG(tmp);
        
        sprintf(tmp,"%f,",global.running_lap_time); DBG(tmp);
        sprintf(tmp,"%f,",global.lap_time); DBG(tmp);
        sprintf(tmp,"%f,",global.best_lap_time); DBG(tmp);
        sprintf(tmp,"%f,",global.delta_time); DBG(tmp);
        sprintf(tmp,"%f,",global.lateral_g); DBG(tmp);
        sprintf(tmp,"%f,",global.linear_g); DBG(tmp);
        sprintf(tmp,"%f,",global.gps_lat); DBG(tmp);
        sprintf(tmp,"%f,",global.gps_lng); DBG(tmp);
        sprintf(tmp,"%d,",global.gps_sats); DBG(tmp);
        sprintf(tmp,"%ld,",global.gps_distance); DBG(tmp);
        sprintf(tmp,"%f\r\n",global.volts); DBG(tmp);
        global.dash_send=0;
    }
}
void VoiceAppMain(void)
{
    unsigned long i=0;
    unsigned char c1;
    unsigned char c2;
    static int l=0;
    char tv[64];
    static unsigned long got_can_norx=0;
    
    
#ifdef DIRECT_RC
  if (direct_rc_link)
  {
      LinkRC();
      return;
  }
#endif
   
      HeartBeat();
    
#ifdef DEMO_LOOP
            DemoLoop();
            HandleSPIFlash();
#else
        HandleSerial();
        HandleStartup();
        HandleCritical();
        HandleGPS();
        HandleLocalAccel();
        HandleDashStream();
        HandleSPIFlash();
        HandleCustomSpeech();
        HandleBluetooth();

#ifdef SPEECH_PING
      if (global.sys_timer>=5)
      {
          sprintf(tv,"LOG %d\r\n",l++);
          Speak(tv,3);
          global.sys_timer=0;
      }
#endif
        if (global.board_version==RACEVOICE_STANDALONE) 
        {
                ProcessTrackFeedback();
                if (GotCAN()) got_can_norx=0; else got_can_norx++;
        }
        else
        {
            if (GotCAN() )
            {
                got_can_norx=0;
                ProcessTrackFeedback();
            }
            else
            {
                got_can_norx++;
            }
        }
        
        #ifndef DEBUG_BLUE_LED
                if (got_can_norx>5000000)    BLUE_LEDOff();
        #endif
#endif
            
   
}

#endif