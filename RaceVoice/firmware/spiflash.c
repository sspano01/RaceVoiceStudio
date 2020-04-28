#ifdef PCMODE
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <math.h>
#include <sys\timeb.h>
#include "defs.h"
#define uint8_t unsigned char
#define uint16_t unsigned short
#define uint32_t unsigned long
#define true 1
#define false 0
#define MAX_NUM_OF_BYTES 256
#else
	#include "app.h"
	#include "defs.h"
#endif

// *****************************************************************************
// *****************************************************************************
// Section: Global Variable Definitions
// *****************************************************************************
// *****************************************************************************
/* EEPROM OP CODES*/
#define EEPROM_WRITE_STATUS_OP_CODE     1
#define EEPROM_WRITE_COMMAND_OP_CODE    2
#define EEPROM_READ_COMMAND_OP_CODE     3
#define EEPROM_READ_STATUS_OP_CODE      5
#define EEPROM_WRITE_ENABLE_OP_CODE     6
#define EEPROM_CHIP_ERASE_OP_CODE     0x60
#define EEPROM_SECTOR_ERASE_OP_CODE     0x20

#define FLASH_TRACE 

#ifdef PCMODE

#define SPI_DATA_TYPE unsigned char
#define DRV_SPI_BUFFER_HANDLE unsigned char

struct
{
	int drvSPIHandle;

	/* SPI Driver Handle  */
	/* Write buffer handle */
	DRV_SPI_BUFFER_HANDLE   drvSPIWRBUFHandle;

	/* Read buffer handle */
	DRV_SPI_BUFFER_HANDLE   drvSPIRDBUFHandle;

	/* SPI Driver TX buffer  */
	SPI_DATA_TYPE           drvSPITXbuffer[MAX_NUM_OF_BYTES<<1];

	/* SPI Driver RX buffer  */
	SPI_DATA_TYPE           drvSPIRXbuffer[MAX_NUM_OF_BYTES<<1];

} appData;


#endif

typedef enum{

    WR_SEND_WRITE_STATUS_CMD = 0,
    WR_WAIT_FOR_STATUS_REPLY,
    WR_SEND_WREN_CMD,
    WR_ENABLE_STATUS_CHECK,
    WR_SEND_WRITE_CMD,
    WR_WAIT_FOR_WRITE_COMPLETE,
    WR_SEND_STATUS_CODE_CMD,
    WR_WAIT_FOR_STATUS_CMD_REPLY,
    WR_GET_STATUS_DATA,
    WR_BUSY_STATUS_CHECK,
    WR_SEND_CHIP_ERASE_CMD,
    WR_COMPLETED

}APP_EEPROM_WR_STATES;

typedef enum{

    RD_IDLE,
    RD_ID,
    RD_SEND_STATUS_CODE_CMD,
    RD_WAIT_FOR_STATUS_CMD_REPLY,
    RD_GET_STATUS_DATA,
    RD_BUSY_STATUS_CHECK,
    RD_SEND_READ_CMD,
    RD_WAIT_FOR_READ_CMD_REPLY,
    RD_GET_DATA,
    RD_WAIT_FOR_DATA,
    RD_COMPLETE

}APP_EEPROM_RD_STATES;

uint8_t APP_EEPROM_Write_Tasks(int);
uint8_t APP_EEPROM_Read_Tasks(void);


uint8_t APP_EEPROM_Check_Transfer_Status(DRV_SPI_BUFFER_HANDLE drvBufferHandle);
void APP_EEPROM_Write( SPI_DATA_TYPE *txbuffer, uint32_t num_of_bytes);
void APP_EEPROM_Read( SPI_DATA_TYPE *rxbuffer, uint32_t num_of_bytes );
extern unsigned long gps_distance(double,double);
// *****************************************************************************
/* Driver objects.

  Summary:
    Contains driver objects.

  Description:
    This structure contains driver objects returned by the driver init routines
    to the application. These objects are passed to the driver tasks routines.
*/

// *****************************************************************************
// *****************************************************************************
// Section: Application Local Routines
// *****************************************************************************
// *****************************************************************************


// *****************************************************************************
// *****************************************************************************
// Section: Application Callback Routines
// *****************************************************************************
// *****************************************************************************



//APP_SPI_CS_DESELECT();
extern struct _global_ global;
extern struct _settings_ settings;
#ifndef PCMODE
extern APP_DATA appData;
#endif
uint8_t getEEPROMdata[MAX_NUM_OF_BYTES] = {0};
extern void DBG(char*);
static char msg[64];
static APP_EEPROM_RD_STATES readstate = RD_IDLE;
int ReadFromFlash(void);
struct _data_  datalog[DATA_LOG_FIFO];
unsigned char* datalog_ptr;
unsigned short DATA_LOG_WATERMARK;
static struct _data_ ldata;

enum
{
    FLASH_IDLE=0,
    FLASH_CHECK,
    FLASH_PRELOAD,
    FLASH_WRITE,
    FLASH_ERASE,
    FLASH_READ,
    FLASH_LAST,
    FLASH_PARK
    
};

static int speech_rd_idx = 0;
static int speech_wr_idx = 0;
#define SPEECH_FIFO 1024
static unsigned char speech_buffer[SPEECH_FIFO];

#define LOG_FIFO_RESET(S)   {global.log_wr_idx=global.log_rd_idx=0; }
#define LOG_FIFO_WRITE(S)     do {                                                                   \
                             global.log_wr_idx = ( (global.log_wr_idx+1)&(DATA_LOG_FIFO-1) ); \
                           }while(0)

#define LOG_FIFO_READ(S)      do {                                                                   \
                             global.log_rd_idx = ( (global.log_rd_idx+1)&(DATA_LOG_FIFO-1) ); \
                           }while(0)

#define LOG_FIFO_LEN()      ( global.log_wr_idx >= global.log_rd_idx ? \
                            global.log_wr_idx - global.log_rd_idx  : \
                            global.log_wr_idx + (DATA_LOG_FIFO - global.log_rd_idx) )


#define SPEECH_FIFO_RESET(S)   {speech_wr_idx=speech_rd_idx=0; }
#define SPEECH_FIFO_WRITE(S)     do {                                                                   \
							 speech_buffer[speech_wr_idx]=S; \
                             speech_wr_idx = ( (speech_wr_idx+1)&(SPEECH_FIFO-1) ); \
                           }while(0)

#define SPEECH_FIFO_READ(S)      do {                                                                   \
						     S=speech_buffer[speech_rd_idx]; \
                             speech_rd_idx = ( (speech_rd_idx+1)&(SPEECH_FIFO-1) ); \
                           }while(0)

#define SPEECH_FIFO_LEN()      ( speech_wr_idx >= speech_rd_idx ? \
                           speech_wr_idx - speech_rd_idx  : \
                            speech_wr_idx + (SPEECH_FIFO - speech_rd_idx) )





#ifdef PCMODE
static volatile unsigned char flash_mem[1024 * 256];
void APP_SPI_CS_DESELECT(void)
{

}

void APP_SPI_CS_SELECT(void)
{

}

void  EraseFlashSegment(int sector)
{

}

void WordPGM(unsigned long addr, unsigned long data)
{
	addr = addr - FLASH_OFFSET - FLASH_BASE;
	
	flash_mem[addr] = (data&0xff000000)>>24;
	flash_mem[addr+1] = (data & 0xff0000) >> 16;
	flash_mem[addr+2] = (data & 0xff00) >> 8;
	flash_mem[addr + 3] = (data & 0xff);
}
#endif

void PushSpeechBuffer(char ch)
{
    if (global.flash_record)
    {
        SPEECH_FIFO_WRITE(ch);
    }
  
}


void WrLogData(void)
{
    static unsigned long sp=0;
    static int log_state=0;
    int index=0;
	static int first = 0;
	static int rec_init = 0;
	int rec_type = 0;
	int i;
	unsigned char ch;
	int ln = 0;
    static int no_move=0;
    static int start_count=0;

#ifdef PCMODE
	if (first == 0)
	{
		memset(flash_mem, 0xff, sizeof(flash_mem));
		for (index = 0;index < 20;index++)
		{
			//flash_mem[index * sizeof(ldata)] = 0x55;
		}
		first = 1;
	}
#endif
	switch (log_state)
	{
	case 0:
		SPEECH_FIFO_RESET();
		//global.flash_last = 1;
		log_state = 1;
        start_count=0;
		break;

	case 1:if (global.flash_last == 0)
			{
				global.flash_wr_addr = global.flash_rd_addr;
				//sprintf(msg,"FLASH:Last @ 0x%08lx\r\n", global.flash_wr_addr); DBG(msg);

			}
		   log_state = 2;
		   break;


	case 2:      // start logging data
        no_move=0;
        start_count=0;
        // if the console manually requests it...
        if (global.flash_record==100 || global.flash_record==101)
        {
            if (global.flash_record==101) global.flash_record=2; else global.flash_record=1;
            log_state=3;
        }
        
        // once the car hits 50mph, we start logging       
        if (global.mph>settings.minmph_threshold && settings.log_enabled==1)
        {
            global.flash_record=1;
            log_state=3;
        }
        
#ifdef FORCE_LOG
		global.flash_record = 2;
		log_state = 3;
#endif          
#ifdef PCMODE
		global.flash_record = 2;
		log_state = 3;
#endif
		break;

	case 3:
        // reprate is 10hz
        if (global.mph<=5)
        {
            
            no_move++;
        }
        else
            no_move=0;
        if (no_move>=(30*10)) global.flash_record=0; // stop recording after 30 seconds
        if (global.flash_record==0) 
            log_state=2; 
        else
		log_state = 3;
		break;
		
	default: log_state = 0; break;
	}

    
    if (global.flash_erase!=0)
    {
        sp=0;
        return;
    }
    
    if (global.flash_record==0)
    {
        sp=0;
        return;
    }

	//printf("Len=%d\r\n", SPEECH_FIFO_LEN());
    
    datalog[global.log_wr_idx].record = DATA_RECORD; // assume we will have a data record
	ln = SPEECH_FIFO_LEN();
	if (ln > 0 && sp>20) // get a few data/init samples before we log speech
	{
		index = 0;

		for (i = 0;i <SPEECH_LOG_SIZE_OVERRIDE;i++)
		{
			datalog[global.log_wr_idx].speech[i] = 0;
		}

		for (i = 0;i < 64;i++)
		{
			SPEECH_FIFO_READ(ch);
			if (ch == 0xaa) break;
			if (ch == 0) continue;
			if (ch == 1) continue;
			if (index < SPEECH_LOG_SIZE_OVERRIDE)
			{ 
				datalog[global.log_wr_idx].speech[index] = ch;
			}
			index++;
		}
        datalog[global.log_wr_idx].record = SPEECH_RECORD;
	}
	else
	{
		for (index = 0;index < SPEECH_LOG_SIZE_OVERRIDE;index++)
		{
			datalog[global.log_wr_idx].speech[index] = 0;
		}
            // don't waste internal logging space, use it only to link speech messages
            // but do a full capture of at least one lap
            //if (global.lapnumber!=4) return;
	}

    if (global.flash_record==2)
    {
        sprintf(msg,"Wr=%d Rd=%d Len=%d Flags=%d,%d\r\n",global.log_wr_idx,global.log_rd_idx,LOG_FIFO_LEN(),rec_init,start_count); DBG(msg);
        
    }
    if(datalog[global.log_wr_idx].record!=SPEECH_RECORD)
    {
        rec_init=1;
        
        if (rec_init && start_count<4)
        {
            for (i = 0;i <SPEECH_LOG_SIZE_OVERRIDE;i++)
            {
                datalog[global.log_wr_idx].speech[i] = 0;
            }
            if (start_count==0)
            {
                datalog[global.log_wr_idx].record = START_RECORD0;
                for (i = 0;i <SPEECH_LOG_SIZE_OVERRIDE;i++)
                {
                    ch= settings.trackname[i];
                    if (i==(SPEECH_LOG_SIZE_OVERRIDE-1)) ch=0;
                    datalog[global.log_wr_idx].speech[i] = ch;
                }
            }
            if (start_count==1)
            {
                datalog[global.log_wr_idx].record = START_RECORD1;
                sprintf(msg,"TRACK=%d\0",settings.trackindex);
                for (i=0;i<strlen(msg);i++)
                datalog[global.log_wr_idx].speech[i]=msg[i];
            }
            if (start_count==2)
            {
                datalog[global.log_wr_idx].record = START_RECORD2;
                sprintf(msg,"%s\0",global.gps_zda);
                for (i=0;i<strlen(msg);i++)
                datalog[global.log_wr_idx].speech[i]=msg[i];
            }
            if (start_count==3)
            {
                datalog[global.log_wr_idx].record = START_RECORD3;
                datalog[global.log_wr_idx].speech[0]='-';
            }
            start_count++;
        }
        else
        {
            datalog[global.log_wr_idx].record = DATA_RECORD;
            datalog[global.log_wr_idx].sample=sp;
            datalog[global.log_wr_idx].lateral=(float)global.lateral_g_raw; // to preserve the sign
            datalog[global.log_wr_idx].linear   =(float)global.linear_g_raw; // to preserve the sign
            datalog[global.log_wr_idx].gps_lat=(float)global.gps_lat;
            datalog[global.log_wr_idx].gps_lng=(float)global.gps_lng;
            datalog[global.log_wr_idx].gps_sats=global.gps_sats;
            datalog[global.log_wr_idx].rpm=(short)global.rpm;
            datalog[global.log_wr_idx].mph=(unsigned char)global.mph;
            datalog[global.log_wr_idx].tps=(unsigned char)global.tps;
            datalog[global.log_wr_idx].lap_time=(float)global.running_lap_time;
            datalog[global.log_wr_idx].lapnumber=(unsigned short)global.lapnumber;
            sp++;
        }
    }
    LOG_FIFO_WRITE(0); // advance pointers
}

int AtEndOfFlash(void)
{
    int theend=0;
    global.flash_max_addr=1024*1024*2;
    if (global.flash_rd_addr>=global.flash_max_addr) theend=1; else theend=0;
    return theend;
}
int FindLast(void)
{
    int more=1;
    int count;
    datalog_ptr=(unsigned char*)&datalog[0];
    for (count=0;count<MAX_NUM_OF_BYTES;count++)
    {
        *datalog_ptr++=getEEPROMdata[count];
    }
   for (count=0;count<DATA_LOG_WATERMARK;count++)
   {
       if (datalog[count].record==0xff)
       {
           more=0;
           break;
       }
   }
    if (AtEndOfFlash())
    {
        more=0;
    }
    return more;
}

int PrintDataLog(void)
{
    int count=0;
    int more=1;
    int i;
    double pos=0;
    char sp[SPEECH_LOG_SIZE_OVERRIDE+2];
    datalog_ptr=(unsigned char*)&datalog[0];
    for (count=0;count<MAX_NUM_OF_BYTES;count++)
    {
        *datalog_ptr++=getEEPROMdata[count];
    }
    pos=((double)global.flash_rd_addr/(double)global.flash_wr_addr)*(double)100;
   for (count=0;count<DATA_LOG_WATERMARK;count++)
   {
       switch(datalog[count].record)
       {
           case NO_RECORD:
               sprintf(msg,"LOG:0x%02x,ENDOFRECORDS",datalog[count].record);
               break;
           case DATA_RECORD:
               
#ifndef PCMODE
               if (global.flash_play)
               {
                        global.gps_lat=datalog[count].gps_lat;
                        global.gps_lng=datalog[count].gps_lng;
                        global.gps_sats=18;
                        global.mph=(int)datalog[count].mph;
                        global.lap_time= datalog[count].lap_time;
                        global.running_lap_time= datalog[count].lap_time;
                        global.gps_update=1;
                        global.gps_ticks++;
                        HandleSegmentsIRQ();
                        HandleGPS();
                        ProcessTrackFeedback();
               }
#endif
               // Current RaceVoice Log format is
               // LOG:0X[RECORDTYPE]
               // Sample#
               // Lapnumber
               // Lattitude
               // Longitude
               // Running Lap Time at the Latt/Longitude
               // Miles Per Hour
               // Engine RPM
               // Engine Throttle Position (TPS)
               // Linear-G Force
               // Lateral-G Force
               // Number of Satellites in use
               // Download percentage, this is just a calculation of how much of the data has been downloaded. Mostly used to make a progress bar move
                 sprintf(msg,"LOG:0x%02x,%d,%d,%f,%f,%f,%d,%d,%d,%f,%f,%d,%d",datalog[count].record,datalog[count].sample,datalog[count].lapnumber,datalog[count].gps_lat,datalog[count].gps_lng,datalog[count].lap_time,datalog[count].mph,datalog[count].rpm,datalog[count].tps,datalog[count].linear,datalog[count].lateral,datalog[count].gps_sats,(int)pos);
                 
                 break;
           case SPEECH_RECORD:
           case START_RECORD0:
           case START_RECORD1:
           case START_RECORD2:
           case START_RECORD3:
               memset(sp,0,sizeof(sp));
               for (i=0;i<SPEECH_LOG_SIZE_OVERRIDE;i++)
               {
                   sp[i]=datalog[count].speech[i];
               }
               sprintf(msg,"LOG:0x%02x,[%s]",datalog[count].record,sp);
               break;
                   
           default:
               sprintf(msg,"LOG:0x%02x,???????",datalog[count].record);
               break;
       }
    if (global.flash_play==0)
    {
     DBG(msg);
     DBG("\r\n"); 
     }
       //if (datalog[count].record==DATA_RECORD)
     //{
     //   sprintf(msg,"Distance=%ld\r\n",gps_distance(datalog[count].gps_lat,datalog[count].gps_lng)); DBG(msg);
    // }
     if (datalog[count].record==0xff ||  AtEndOfFlash())
     {
         DBG("LOG:COMPLETE\r\n");
         more=0;
         break;
     }
   }
    
    return more;
    
}

void FlashStatus(void)
{
    if (global.internal_log)
    {
      sprintf(msg,"\r\nFLASH: Erase=%d Wr=%ld Rd=%d Fifo=%d StructSize=%d Id=Internal\r\n",global.flash_erase,global.flash_wr_addr,global.flash_rd_addr,LOG_FIFO_LEN(),sizeof(ldata));
    }
    else
      sprintf(msg,"\r\nFLASH: Erase=%d Wr=%ld Rd=%d Fifo=%d StructSize=%d Id=0x%02x,0x%02x,0x%02x\r\n",global.flash_erase,global.flash_wr_addr,global.flash_rd_addr,LOG_FIFO_LEN(),sizeof(ldata),global.flash_id[0],global.flash_id[1],global.flash_id[2]);
  DBG(msg);
    
}
void TraceSpiBuffer(int read)
{
    int count=0;
    int index=0;
    if (read)
    {
        sprintf(msg,"\r\nRD [0x%08lx]:\r\n",global.flash_rd_addr); DBG(msg);
        for (count=0;count<MAX_NUM_OF_BYTES;count++)
        {
                sprintf(msg,"0x%02x \0",getEEPROMdata[count]); DBG(msg);
                index++;
                if (index==16)
                {
                    index=0;
                    DBG("\r\n");
                }
        }
    }
    
    else
    {
      
        DBG("WR:\0");
        for (count=0;count<MAX_NUM_OF_BYTES;count++)
        {
                sprintf(msg,"0x%02x \0",getEEPROMdata[count]); DBG(msg);
                index++;
                if (index==16)
                {
                    index=0;
                    DBG("\r\n");
                }
        }  
    }
    
}

int ValidSpiFlash(void)
{
    int valid=0;
    
    // Macronix MX25L1606 16Mb flash
    if (global.flash_id[0]==0xc2 && global.flash_id[1]==0x20 && global.flash_id[2]==0x15)
    {   
        settings.log_enabled=1;
        valid=1;
    }
    if (global.internal_log) valid=1;
    
#ifdef FORCE_INTERNAL
    global.internal_log=1;
#endif
    return valid;
    
}

void EraseInternalFlash(void)
{
     EraseFlashSegment(DSECTOR);
}

void WriteInternalFlash(void)
{
    int cnt;
    unsigned long word;
    unsigned long addr=0;
	//printf("Size=%ld\r\n",sizeof(ldata));
    for (cnt=0;cnt<MAX_NUM_OF_BYTES;cnt=cnt+4)
    {
        word=(((unsigned long)getEEPROMdata[cnt])<<24);
        word|=(((unsigned long)getEEPROMdata[cnt+1])<<16);
        word|=(((unsigned long)getEEPROMdata[cnt+2])<<8);
        word|=(((unsigned long)getEEPROMdata[cnt+3]));
        
        if (global.flash_wr_addr>=FLASH_SIZE)
        {
            DBG("FLASH:Internal Record Stopped\r\n");
            global.flash_record=0;
            return;
        }
        addr=global.flash_wr_addr+FLASH_OFFSET+FLASH_BASE;
        WordPGM(addr,word);
       // sprintf(msg,"0x%08lx 0x%08lx\r\n",addr,word); DBG(msg);
        
        global.flash_wr_addr+=4;
    }

}

int ReadFromInternalFlash(void)
{
    int cnt;
    unsigned long raddr=0;
    unsigned long rdval=0;
    for (cnt=0;cnt<MAX_NUM_OF_BYTES;cnt=cnt+4)
    {
		#ifdef PCMODE
		raddr = global.flash_rd_addr;
		rdval = flash_mem[raddr] << 24 | flash_mem[raddr + 1] << 16 | flash_mem[raddr + 2] << 8 | flash_mem[raddr+3];
		#else
		
		raddr=FLASH_BASE_RD+FLASH_OFFSET+global.flash_rd_addr;
        if (global.flash_rd_addr>=FLASH_SIZE)
        {
            rdval=0xffffffff;
        }
        else
        {
            rdval=*(volatile unsigned long*)raddr;
        }
		#endif
		global.flash_rd_addr += 4;

       // sprintf(msg,"RD @ 0x%08lx = 0x%08lx\r\n",raddr,rdval); DBG(msg);
        getEEPROMdata[cnt]=(rdval&0xff000000)>>24;
        getEEPROMdata[cnt+1]=(rdval&0xff0000)>>16;
        getEEPROMdata[cnt+2]=(rdval&0xff00)>>8;
        getEEPROMdata[cnt+3]=(rdval&0xff);
    }
    
    return 0;
}


void HandleSPIFlash(void)
{
    //APP_EEPROM_Write_Tasks();
    static int spi_state=0;
    static int cnt=0;
    static int noflash=0;
    int readit=1;
    int st=0;
	static int init = 0;
    static int por=0;
	char sp[36];
	int i;
    static unsigned long flash_steps=0;
    static int last_state=-1;
    
    if (global.board_version!=RACEVOICE_STANDALONE) return;
  
#if(0)
    if (last_state!=spi_state)
    {
     sprintf(msg,"SpiState=%d\r\n",spi_state); DBG(msg);
     last_state=spi_state;
    }
#endif
    switch(spi_state)
    {
        case FLASH_IDLE: 
			init = 0;
            DATA_LOG_WATERMARK=MAX_NUM_OF_BYTES/sizeof(ldata);
            APP_EEPROM_Write_Tasks(0);
            flash_steps=0;
            if (LOG_FIFO_LEN()>DATA_LOG_WATERMARK)
            {
                spi_state=FLASH_PRELOAD;
                break;
            }
            if (global.read_from_flash || global.read_from_flash_step || global.flash_play)
            {
                spi_state=FLASH_READ;
            }
            
            if (global.flash_erase)
            {
                spi_state=FLASH_ERASE;
            }
            
            if (global.flash_check)
            {
                spi_state=FLASH_CHECK;
            }
            if (global.flash_last)
            {
                spi_state=FLASH_LAST;
            }
            
            if (por==0)
            {
                por=1;
                spi_state=FLASH_CHECK;
            }
            break;
        
        case FLASH_PRELOAD:
            
            datalog_ptr=(unsigned char*)&datalog[global.log_rd_idx];
            for (cnt=0;cnt<DATA_LOG_WATERMARK;cnt++)
            {
                if (global.flash_record==2)
                {
					memset(sp, 0, sizeof(sp));
                    if (datalog[global.log_rd_idx].record==SPEECH_RECORD)
                    {
                        for (i = 0;i < SPEECH_LOG_SIZE_OVERRIDE;i++) sp[i] = datalog[global.log_rd_idx].speech[i];
                        sprintf(msg,"SpeechLog=[%s]\r\n",sp); 
                    }
                    else
                    {
                        sprintf(msg,"Sample=%d,%f,%f,%d,%d,%f\r\n",datalog[global.log_rd_idx].sample,datalog[global.log_rd_idx].gps_lat,datalog[global.log_rd_idx].gps_lng,datalog[global.log_rd_idx].rpm,datalog[global.log_rd_idx].mph,datalog[global.log_rd_idx].lap_time);
                    }
                    DBG(msg);
                }
                LOG_FIFO_READ();
            }
            for (cnt=0;cnt<MAX_NUM_OF_BYTES;cnt++)
            {
                 getEEPROMdata[cnt]=*datalog_ptr++;   
            }
            if (global.flash_record==2)
            {
                sprintf(msg,"WR @ 0x%08lx Rd=0x%08lx\r\n",global.flash_wr_addr,global.flash_rd_addr); DBG(msg);
            }
            spi_state=FLASH_WRITE;
            break;
        case FLASH_WRITE:
            if (global.internal_log)
            {
                WriteInternalFlash();
                spi_state=FLASH_IDLE;
            }
            else
            {
                if(APP_EEPROM_Write_Tasks(1)) 
                {
                    global.flash_wr_addr+=MAX_NUM_OF_BYTES;
                   spi_state=FLASH_IDLE;
                }
            }
            break;
        case FLASH_ERASE:
            LOG_FIFO_RESET();
            global.flash_wr_addr=0;
            global.flash_rd_addr=0;
            global.flash_erase=2;
            if (global.internal_log)
            {
                EraseInternalFlash();
                global.flash_erase=0;
                spi_state=FLASH_IDLE;
            }
            else
            {    
                if(APP_EEPROM_Write_Tasks(2)) 
                {
                    global.flash_erase=0;
                    spi_state=FLASH_IDLE;
                }
            }
            break;
        case FLASH_READ:
            DBG("\r\n");
            //gps_distance(0,0);
            while(readit)
            {
                if (global.internal_log)
                {
                    st=ReadFromInternalFlash();
                }
                else
                {
                    st=ReadFromFlash();
					global.flash_rd_addr += MAX_NUM_OF_BYTES;
				}
                if (global.read_from_flash==2)
                {
                    TraceSpiBuffer(1);
                }
                readit=PrintDataLog();
                
                if (readit!=0)
                {
                    if (global.read_from_flash_step)
                    {
                        flash_steps+=MAX_NUM_OF_BYTES;
                        if (flash_steps>=FLASH_STEP_SIZE) readit=0;
                    }
                }
            }
            global.read_from_flash_step=0;
            global.read_from_flash=0;
            global.flash_play=0;
            spi_state=FLASH_IDLE;
            break;
        case FLASH_LAST:
            global.flash_rd_addr=0;
            global.flash_init=0;
            while(readit)
            {
                if (global.internal_log)
                    st=ReadFromInternalFlash();
                else
                    st=ReadFromFlash();

				readit=FindLast();
                //RotateLed();
                // sprintf(msg,"ReadAt=0x%08lx\r\n",global.flash_rd_addr); DBG(msg);
              

                if (readit && init)
                {
                  global.flash_rd_addr+=MAX_NUM_OF_BYTES;
                }
				if (init == 0 && readit == 0)
				{
					global.flash_rd_addr = 0;
				}
                
				init = 1;
            }
            global.flash_init=1;
            global.flash_wr_addr=global.flash_rd_addr;
            global.flash_last=0;
            spi_state=FLASH_IDLE;
            break;
        case FLASH_CHECK:
            datalog_ptr=(unsigned char*)&datalog[0];
            LOG_FIFO_RESET(0);
            global.read_from_flash_step=global.flash_erase=global.flash_record=global.read_from_flash=0;
            global.flash_play=0;
            global.flash_rd_addr=global.flash_wr_addr=0;
            global.flash_check=0;
#ifdef PCMODE
			spi_state = FLASH_IDLE;
			global.internal_log = 1;
			break;
#endif
            //sprintf(msg,"Check=%d\r\n",noflash); DBG(msg);
            st=CheckSPIFlash(noflash);
            if (!st)
            {
                noflash++;
                if (noflash>100)
                {
                    
                    spi_state=FLASH_PARK;
                    //global.internal_log=1;
                    //spi_state=FLASH_IDLE;
                }
                else
                {
                    spi_state=FLASH_CHECK;
                }
            }
            else 
            {
                global.flash_last=1;
                spi_state=FLASH_IDLE;
            }
            break;
        case FLASH_PARK:
            settings.log_enabled=0;
            break;
        default:spi_state=FLASH_CHECK;
                cnt=0;
                break;
    }
    
}


int ReadFromFlash(void)
{
     uint8_t st=0;
        /* Add the EEPROM Read STATUS OP Code to the buffer*/
         readstate=RD_SEND_READ_CMD;
         while(1)
         {
           st=APP_EEPROM_Read_Tasks();  
           if (st==true) break;
         }
         return 1; 
    
}

int CheckSPIFlash(void)
{
        uint8_t st=0;
        /* Add the EEPROM Read STATUS OP Code to the buffer*/
         //DBG("Checking For SPI Flash....\r\n");
         appData.drvSPITXbuffer[0] = 0x9f; // read back the jedec 
         appData.drvSPITXbuffer[1] = 0; /* Address - LSB */
         appData.drvSPITXbuffer[2] = 0; /* Address - MSB */
         appData.drvSPITXbuffer[3] = 0; /* Dummy byte */
         readstate=RD_ID;
         while(1)
         {
           st=APP_EEPROM_Read_Tasks();  
           if (st==true) 
           {
               break;
           }
         }
         
        for (st=0;st<3;st++) global.flash_id[st]=getEEPROMdata[st];
        if (ValidSpiFlash())
        {
            sprintf(msg,"FLASH:ManufID=0x%02x DeviceID=0x%02x Density=0x%02x\r\n",getEEPROMdata[0],getEEPROMdata[1],getEEPROMdata[2]);
            DBG(msg);
            return 1;
        }
        else
        {
           // sprintf(msg,"NOT MAPPED FLASH:ManufID=0x%02x DeviceID=0x%02x Density=0x%02x\r\n",getEEPROMdata[0],getEEPROMdata[1],getEEPROMdata[2]);
           // DBG(msg);
        }
 
         
         return 0;
         
}



void APP_EEPROM_Read( SPI_DATA_TYPE *rxbuffer, uint32_t num_of_bytes )
{
#ifdef PCMODE

#else
	/* Add the buffer pointer to read the data from EEPROM */
    appData.drvSPIRDBUFHandle = DRV_SPI_BufferAddRead( appData.drvSPIHandle,
                    (SPI_DATA_TYPE *)&appData.drvSPIRXbuffer[0], num_of_bytes, 0, 0);
#endif

}

uint8_t APP_EEPROM_Read_Tasks(void)
{
    uint32_t num_of_bytes;
    uint32_t count;
    static int bytes_to_read=0;
    int index=0;
    
    switch(readstate)
    {
        case RD_IDLE:
                APP_SPI_CS_DESELECT();
        break;
        
        case RD_ID:
             /* Add read command op code & 16-bit beginning address to buffer. 
             * This sequence picks the data byte from the selected location and 
             * holds it in the shift register of the SPI interface of the EEPROM */
            APP_SPI_CS_DESELECT();
            appData.drvSPITXbuffer[0] = 0x9F;
            appData.drvSPITXbuffer[1] = 0; /* Address - LSB */
            appData.drvSPITXbuffer[2] = 0; /* Address - MSB */
            /* A dummy byte is needed to push that byte of data out of shift register*/
            appData.drvSPITXbuffer[3] = 0; /* Dummy byte */

            /* Number of bytes to transfer */
            num_of_bytes = 4;
            bytes_to_read=3;

            /* Assert the CS line */
            APP_SPI_CS_SELECT();

            /* Add to the write buffer and transmit */
            APP_EEPROM_Write(&appData.drvSPITXbuffer[0], num_of_bytes);
            readstate = RD_WAIT_FOR_READ_CMD_REPLY;
            break;
            
        /* Send read status commmand to check EEPROM is busy for not! */
        case RD_SEND_STATUS_CODE_CMD:
        {
            /* Assert the CS Line */
            APP_SPI_CS_SELECT();

            /* Add the EEPROM Read STATUS OP Code to the buffer*/
            appData.drvSPITXbuffer[0] = EEPROM_READ_STATUS_OP_CODE;
            appData.drvSPITXbuffer[1] = 0; /* Dummy byte */

            /* Number of bytes to transfer */
            num_of_bytes = 2;

            /* Add to the write buffer to transmit*/
            APP_EEPROM_Write(&appData.drvSPITXbuffer[0], num_of_bytes);
            readstate = RD_WAIT_FOR_STATUS_CMD_REPLY;

            break;
        }
        case RD_WAIT_FOR_STATUS_CMD_REPLY:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIWRBUFHandle))
            {
                /* Transfer Status Success*/
                readstate = RD_GET_STATUS_DATA;
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                readstate = RD_WAIT_FOR_STATUS_CMD_REPLY;
            }
            break;
        }
        case RD_GET_STATUS_DATA:
        {
            /* Add the buffer to get the data from EEPROM*/
            num_of_bytes = 1;

            APP_EEPROM_Read(&appData.drvSPIRXbuffer[0], num_of_bytes);
            readstate = RD_BUSY_STATUS_CHECK;

            break;
        }
        case RD_BUSY_STATUS_CHECK:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIRDBUFHandle))
            {
                /* Deassert the CS Line*/
                APP_SPI_CS_DESELECT();

                //Check if Write in Progress (WIP) is true
                if(appData.drvSPIRXbuffer[0] & 0x01)
                {
                    /* Re-Send status command again to check the busy stautus */
                    readstate = RD_SEND_STATUS_CODE_CMD;
                }
                else
                {
                    /* Transmit read command to read data from EEPROM memory */
                    readstate = RD_SEND_READ_CMD;
                }
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                readstate = RD_BUSY_STATUS_CHECK;
            }
            break;
        }
        case RD_SEND_READ_CMD:
        {
            /* Add read command op code & 16-bit beginning address to buffer. 
             * This sequence picks the data byte from the selected location and 
             * holds it in the shift register of the SPI interface of the EEPROM */
            appData.drvSPITXbuffer[0] = EEPROM_READ_COMMAND_OP_CODE;
            appData.drvSPITXbuffer[1] = (global.flash_rd_addr & 0xff0000)>>16; /* Address - LSB */
            appData.drvSPITXbuffer[2] = (global.flash_rd_addr & 0xff00)>>8; /* Address - MSB */
            /* A dummy byte is needed to push that byte of data out of shift register*/
            appData.drvSPITXbuffer[3] = (global.flash_rd_addr & 0xff); /* Dummy byte */

            /* Number of bytes to transfer */
            num_of_bytes = 4;
            bytes_to_read=MAX_NUM_OF_BYTES;

            /* Assert the CS line */
            APP_SPI_CS_SELECT();

            /* Add to the write buffer and transmit */
            APP_EEPROM_Write(&appData.drvSPITXbuffer[0], num_of_bytes);
            readstate = RD_WAIT_FOR_READ_CMD_REPLY;

            break;
        }
        case RD_WAIT_FOR_READ_CMD_REPLY:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIWRBUFHandle))
            {
                /*  Get the data from EEPROM */
                readstate = RD_GET_DATA;
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                readstate = RD_WAIT_FOR_READ_CMD_REPLY;
            }
            break;
        }
        case RD_GET_DATA:
        {
            /* Add the buffer pointer to read the data*/
            APP_EEPROM_Read(&appData.drvSPIRXbuffer[0], bytes_to_read);
            readstate = RD_WAIT_FOR_DATA;

            break;
        }
        case RD_WAIT_FOR_DATA:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIRDBUFHandle))
            {
                /* Deassert the CS Line */
                APP_SPI_CS_DESELECT();

                readstate = RD_COMPLETE;
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                readstate = RD_WAIT_FOR_DATA;
            }
            break;
        }
        case RD_COMPLETE:
        {
            /* Copy the received data to the local buffer */
            index=0;
            for(count = 0; count < bytes_to_read;)
            {
                getEEPROMdata[count] = appData.drvSPIRXbuffer[count];
                count++;
            }
            
#ifdef SHOW_RD_BUFFER
            TraceSpiBuffer(1);
#endif

            /* return done to app task */
            return true;
            break;
        }
        default:
            break;
    }

    return false;
}


uint8_t APP_EEPROM_Write_Tasks(int state)
{
    static APP_EEPROM_WR_STATES writestate = WR_SEND_WREN_CMD;
    uint32_t num_of_bytes, loop, dataCount;

    if (state==0)
    {
        writestate = WR_SEND_WREN_CMD;
        return 0;
    }
#ifdef TRACE_SPI
    sprintf(msg,"WriteTask=%d\r\n",writestate); 
    DBG(msg);
#endif
    switch(writestate)
    {
            
        case WR_SEND_WREN_CMD:
        {
            /* Assert CS Line */
            APP_SPI_CS_SELECT();

            /* Add Write Enable op code to the buffer */
            appData.drvSPITXbuffer[0] = EEPROM_WRITE_ENABLE_OP_CODE;

            /* Number of bytes to transfer */
            num_of_bytes = 1;

            /* Add to the write buffer and transmit*/
            APP_EEPROM_Write(&appData.drvSPITXbuffer[0], num_of_bytes);
            writestate = WR_ENABLE_STATUS_CHECK;

            break;
        }
        case WR_ENABLE_STATUS_CHECK:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIWRBUFHandle))
            {
                /* Deassert CS Line */
                APP_SPI_CS_DESELECT();

                if (state==2)
                {
                    writestate = WR_SEND_CHIP_ERASE_CMD;
                }
                else
                {
                    writestate = WR_SEND_WRITE_CMD;
                }
                //writestate=WR_SEND_WREN_CMD;
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                writestate = WR_ENABLE_STATUS_CHECK;
            }
            break;
        }
        
        case WR_SEND_CHIP_ERASE_CMD:
        {
            APP_SPI_CS_SELECT();

            appData.drvSPITXbuffer[0] = EEPROM_CHIP_ERASE_OP_CODE;

            dataCount = 1;

            /* Add the write buffer and transmit*/
            APP_EEPROM_Write(&appData.drvSPITXbuffer[0], dataCount);
            writestate = WR_WAIT_FOR_WRITE_COMPLETE;
            break;
        }
                
        case WR_SEND_WRITE_CMD:
        {
            APP_SPI_CS_SELECT();

            appData.drvSPITXbuffer[0] = EEPROM_WRITE_COMMAND_OP_CODE;
            appData.drvSPITXbuffer[1] = (global.flash_wr_addr & 0xff0000)>>16; /* Address - MSB */
            appData.drvSPITXbuffer[2] = (global.flash_wr_addr & 0xff00)>>8; /* Address - LSB */
            appData.drvSPITXbuffer[3] = (global.flash_wr_addr & 0xff); /* Dummy byte */

            dataCount = 4;

            /* Add the data to the buffer */
            for(loop =0; loop < MAX_NUM_OF_BYTES; loop++)
            {
                    appData.drvSPITXbuffer[dataCount] = getEEPROMdata[loop];
                    dataCount++;
            }
            
            /* Number of bytes to transfer */
            num_of_bytes = dataCount; //opcode + address + data

            /* Add the write buffer and transmit*/
            APP_EEPROM_Write(&appData.drvSPITXbuffer[0], num_of_bytes);
            writestate = WR_WAIT_FOR_WRITE_COMPLETE;
            break;
        }
        case WR_WAIT_FOR_WRITE_COMPLETE:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIWRBUFHandle))
            {
                APP_SPI_CS_DESELECT();

                writestate = WR_SEND_STATUS_CODE_CMD;
                //writestate=WR_SEND_WRITE_CMD;
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                writestate = WR_WAIT_FOR_WRITE_COMPLETE;
            }
            break;
        }
        /* Send read status commmand to check EEPROM is busy for not! */
        case WR_SEND_STATUS_CODE_CMD:
        {
            /* Assert the CS Line */
            APP_SPI_CS_SELECT();

            /* Add the EEPROM Read STATUS OP Code to the buffer*/
            appData.drvSPITXbuffer[0] = EEPROM_READ_STATUS_OP_CODE;
            appData.drvSPITXbuffer[1] = 0;

            /* Number of bytes to transfer */
            num_of_bytes = 2;

            /* Add to the write buffer to transmit*/
            APP_EEPROM_Write(&appData.drvSPITXbuffer[0], num_of_bytes);
            writestate = WR_WAIT_FOR_STATUS_CMD_REPLY;

            break;
        }
        case WR_WAIT_FOR_STATUS_CMD_REPLY:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIWRBUFHandle))
            {
                /* Transfer Status Success*/
                writestate = WR_GET_STATUS_DATA;
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                writestate = WR_WAIT_FOR_STATUS_CMD_REPLY;
            }
            break;
        }
        case WR_GET_STATUS_DATA:
        {
            /* Add the buffer to get the data from EEPROM*/
            num_of_bytes = 1;

            APP_EEPROM_Read(&appData.drvSPIRXbuffer[0], num_of_bytes);
            writestate = WR_BUSY_STATUS_CHECK;

            break;
        }
        case WR_BUSY_STATUS_CHECK:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIRDBUFHandle))
            {
                /* Deassert the CS Line*/
                  APP_SPI_CS_DESELECT();
                //Check if Write in Progress (WIP) is true
#ifdef TRACE_SPI
                  sprintf(msg,"Status=0x%02x\r\n",appData.drvSPIRXbuffer[0]); DBG(msg);
#endif
                if(appData.drvSPIRXbuffer[0] & 0x01)
                {
                    /* Re-Send status command again to check the busy stautus */
                    writestate = WR_SEND_STATUS_CODE_CMD;
                }
                else
                {
                    /* Transmit read command to read data from EEPROM memory */
                    writestate = WR_COMPLETED;
                   //  writestate = WR_SEND_STATUS_CODE_CMD;
                }
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                writestate = WR_BUSY_STATUS_CHECK;
            }
            break;
        }
        case WR_COMPLETED:
        {
            /* return done to app task */
            //writestate = WR_SEND_WRITE_STATUS_CMD;
            return true;
            break;
        }
        default:
            break;
    }
    return false;
}




#if(0)
uint8_t APP_EEPROM_Write_Tasks(int state)
{
    static APP_EEPROM_WR_STATES writestate = WR_SEND_WRITE_STATUS_CMD;
    uint32_t num_of_bytes, loop, dataCount;

    if (state==0)
    {
        writestate = WR_SEND_WRITE_STATUS_CMD;
        return 0;
    }
    sprintf(msg,"WriteTask=%d\r\n",writestate); 
    DBG(msg);
    switch(writestate)
    {
        case WR_SEND_WRITE_STATUS_CMD:
        {
            /* Assert CS Line */
            APP_SPI_CS_SELECT();

            /* Add Write status op code, data to the buffer */
            appData.drvSPITXbuffer[0] = EEPROM_WRITE_STATUS_OP_CODE;
            appData.drvSPITXbuffer[1] = 0x08; /* EEPROM BP1 = 1, BP0 = 0 */
            appData.drvSPITXbuffer[2] = 0; /* Dummy byte */

            /* Number of bytes to transfer */
            num_of_bytes = 3;

            /* Add to the write buffer and transmit*/
            APP_EEPROM_Write(&appData.drvSPITXbuffer[0], num_of_bytes);
            writestate = WR_WAIT_FOR_STATUS_REPLY;

            break;
        }
        case WR_WAIT_FOR_STATUS_REPLY:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIWRBUFHandle))
            {
                /* Deassert CS Line */
                APP_SPI_CS_DESELECT();

                writestate = WR_SEND_WREN_CMD;
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                writestate = WR_WAIT_FOR_STATUS_REPLY;
            }
            break;
        }
        case WR_SEND_WREN_CMD:
        {
            /* Assert CS Line */
            APP_SPI_CS_SELECT();

            /* Add Write Enable op code to the buffer */
            appData.drvSPITXbuffer[0] = EEPROM_WRITE_ENABLE_OP_CODE;
            appData.drvSPITXbuffer[1] = 0; /* Dummy byte */

            /* Number of bytes to transfer */
            num_of_bytes = 2;

            /* Add to the write buffer and transmit*/
            APP_EEPROM_Write(&appData.drvSPITXbuffer[0], num_of_bytes);
            writestate = WR_ENABLE_STATUS_CHECK;

            break;
        }
        case WR_ENABLE_STATUS_CHECK:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIWRBUFHandle))
            {
                /* Deassert CS Line */
                APP_SPI_CS_DESELECT();

                writestate = WR_SEND_WRITE_CMD;
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                writestate = WR_ENABLE_STATUS_CHECK;
            }
            break;
        }
        case WR_SEND_WRITE_CMD:
        {
            APP_SPI_CS_SELECT();

            appData.drvSPITXbuffer[0] = EEPROM_WRITE_COMMAND_OP_CODE;
            appData.drvSPITXbuffer[1] = 0; /* Address - MSB */
            appData.drvSPITXbuffer[2] = 0; /* Address - LSB */
            appData.drvSPITXbuffer[3] = 0; /* Dummy byte */

            dataCount = 4;

            /* Add the data to the buffer */
            for(loop =0; loop < MAX_NUM_OF_BYTES; )
            {
                if(loop%2)
                {
                    appData.drvSPITXbuffer[dataCount++] = 0x55;
                }
                else
                {
                    appData.drvSPITXbuffer[dataCount++] = 0xAA;
                }

                loop++;
            }
            /* Number of bytes to transfer */
            num_of_bytes = dataCount; //opcode + address + data

            /* Add the write buffer and transmit*/
            APP_EEPROM_Write(&appData.drvSPITXbuffer[0], num_of_bytes);
            writestate = WR_WAIT_FOR_WRITE_COMPLETE;

            break;
        }
        case WR_WAIT_FOR_WRITE_COMPLETE:
        {
            /* Check if the transfer status is success or not */
            if(APP_EEPROM_Check_Transfer_Status(appData.drvSPIWRBUFHandle))
            {
                APP_SPI_CS_DESELECT();

                writestate = WR_COMPLETED;
            }
            else
            {
                /* Transfer Status - Wait untill staus becomes true*/
                writestate = WR_WAIT_FOR_WRITE_COMPLETE;
            }
            break;
        }
        case WR_COMPLETED:
        {
            /* return done to app task */
            writestate = WR_SEND_WRITE_STATUS_CMD;
//return true;
            break;
        }
        default:
            break;
    }
    return false;
}

#endif

void APP_EEPROM_Write( SPI_DATA_TYPE * txbuffer, uint32_t num_of_bytes)
{
    
#ifdef PCMODE
	
#else
	
	/* Add the buffer to transmit */
    appData.drvSPIWRBUFHandle = DRV_SPI_BufferAddWrite(appData.drvSPIHandle,
                     (SPI_DATA_TYPE *)&appData.drvSPITXbuffer[0], num_of_bytes, 0, 0);
#endif
}

uint8_t APP_EEPROM_Check_Transfer_Status(DRV_SPI_BUFFER_HANDLE drvBufferHandle)
{
#ifdef PCMODE
	return true;
#else
    if(DRV_SPI_BUFFER_EVENT_COMPLETE & DRV_SPI_BufferStatus (drvBufferHandle))
    {
        return true;
    }
    else
    {
        return false;
    }
#endif
}

/*******************************************************************************
 End of File
 */
