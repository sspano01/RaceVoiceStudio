
//#define JIM
//#define BETA 1
//#define STEVE
//#define SMARTY_CAM

// disable no connection and gps error reporting
// use for testing only
//#define DEBUG_BLUE_LED 1
//#define GPS_MATCH_TIMING 1
//#define GPS_IRQ_TIMING 1



//#define AUDIO_LOOP
#define VERSION "April 19, 2020, A1"
#define NO_BACKGROUND_ERROR 1


//#define FORCE_OBDII
//#define FORCE_CUSTOM_CAN
//#define FORCE_LOG 
//#define SPEECH_PING
//#define FORCE_INTERNAL 
//#define ALLOW_SPIFLASH

#define _125K_ 125
#define _250K_ 250
#define _500K_ 500
#define _1000K_ 1000

#define MIN_ANNOUNCE_VOLTS 6
#define _READYFLAG_ 0x1234ABCD
//#define FORCE_BAUD _500K_

#define DISABLE_IRQ() asm volatile ("di")
#define ENABLE_IRQ() asm volatile ("ei")

//#define DISABLE_CANSENSOR

//#define CAN_DEBUG 1

#define LED_ON_RATE 10000000
#define RXB_SIZE 8
#define TXB_SIZE 8
#define FLASH_SETTINGS_ADDRESS 0x9d070000

// timer value for ~10hz file stepping
#define PLAY_RATE 1500 

// number of gps messages that we need to get which do not match the start finish location
// so that we set the lap timer to trigger again
#define LAP_HYST 100


//#define NO_VOICE 1
//#define SPEECH_TEST 1
//#define DEMO_LOOP 1

#define DSECTOR 1
#define RSECTOR 0

#define BOOT_FLASH_SIZE     0x2FEF
#define BOOT_FLASH_BASE_RD 0xBFC00000
#define BOOT_FLASH_BASE    0x1FC00000

#define FLASH_BASE_RD 0xBD000000
#define FLASH_BASE    0x1D000000

#define GPS_INTEROPLATE_STEPS 20

// 1d07:c000 --> this is where the flash memory write routines are located based on the linker file
// we should NEVER be changing this during a firmware download
#define RESERVED_FLASHSECTION (FLASH_BASE+0x7C000)

// last 16k reserved for memory locked functions
#define FLASH_SIZE    (240*1024)
#define FLASH_OFFSET  (256*1024)
#define DRV_NVM_PROGRAM_UNLOCK_KEY1 0xAA996655
#define DRV_NVM_PROGRAM_UNLOCK_KEY2 0x556699AA

#define MAX_SEGMENTS 10
#define MAX_PHRASE 8
#define PHRASE_LEN 32
//#define LAP_SIM 1
//#define TIRE_SIM 1

#define TPS_OBD_HIGH_DEFAULT 50
#define TPS_HIGH_DEFAULT 90

// Canbus Message Maps
#define CAN_ENGINE_STAT1 0x50
#define CAN_WHEEL_SPEED 0x51
#define CAN_PHRASES 0x52
#define CAN_LAT_G 0x53
#define CAN_ENGINE_STAT2 0x55
#define CAN_GPS 0x56
#define CAN_VOICE_SETTING 0x57
#define CAN_LAP_STATUS  0x5A
#define CAN_DEBUG_FLAG 0x5F
#define CAN_TPMS 0x433
#define CAN_VBOX 0x30

#define AUTOSPORTS_A10L 0xa100
#define AUTOSPORTS_A10H 0xa101

// SmartyCam Emulation
//https://wiki.autosportlabs.com/AIM_Integration_CAN_Mapping
#define SMARTY_GPS 40
#define SMARTY_1056 1056
#define SMARTY_1057 1057
#define SMARTY_1058 1058
#define SMARTY_1059 1059
#define SMARTY_1060 1060

// VBOX HD2
//https://racelogic.support/02VBOX_Motorsport/Video_Data_Loggers/VBOX_Video_HD2/VBOX_Video_HD2_User_Guide/10_-_HD2_Technical_Properties/HD2_-_CAN_Output_Format
#define VBOX_LAT 0x301
#define VBOX_LNG_VEL 0x302
#define VBOX_GFORCE  0x304
#define VBOX_LAP  0x7e1

#define MPH_TRIGGERS 5

#define SCALE_RPM 100
#define SCALE_MPH 101
#define SCALE_TPS 102
#define SCALE_BRAKE 103
#define SCALE_OIL 104
#define SCALE_TEMP 105
#define SCALE_DISTANCE 106
#define SCALE_GPS 107
#define SCALE_VOLTS 108
#define SCALE_LAPTIME  109
#define INTERNAL_LAP_TIMER 110
#define SCALE_MPH_D10_MM 111
#define SCALE_TEMP_C 112
#define SCALE_MPH_D10_KM 113
#define SCALE_BAR_TO_PSI 114
#define SCALE_LAT_G_RAW 115

#define CAN_TIRE_LF 0x70
#define CAN_TIRE_RF 0x71
#define CAN_TIRE_LR 0x72
#define CAN_TIRE_RR 0x73


#define TRACE_CAN 0x01
#define TRACE_CAN_VALUES 0x02
#define INJECT_OIL_ERROR 0x80
#define INJECT_TEMPERATURE_ERROR 0x100

#define UNIT_NAME_SIZE 32
#define VALID_KEY 0x1234ABCD

#define FLASH_STEP_SIZE 0x8000

// bit bectors
// these need to match the order of the
// checkbox values in the mainform.cs datacheckboxes
#define EN_MINIMUM_SPEED 0x01
#define EN_ENTRY_SPEED 0x02
#define EN_EXIT_SPEED 0x04
#define EN_TURN_IN_SPEED 0x08
#define EN_MAX_LATERAL 0x10
#define EN_MAX_LINEAR 0x20
#define EN_ROLLING_LATERAL 0x40
#define EN_SEGMENT_MPH 0x80

#define SPLITS 1
#define SEGMENT_START 2
#define SEGMENT_STOP 4
#define CHECKER 8
#define CUSTOM_SPEECH_LEN 16
#define GPS_ZDA_LEN 32
#ifdef PCMODE
    #define TRACE_GPS 6
#endif
enum
{
    LOW,
    HIGH,
    IMMEDIATE
};


enum
{
    AIM,
    MOTEC,
    SMARTY,
    VBOX,
    AUTOSPORTS,
    STANDALONE,
    OBD_II,
    NONE6,
    NONE7,
    NONE8,
    NONE9,
    NONE10,
    NONE11,
    NONE12,
    NONE13,
    NONE14,
    CUSTOM_CAN
};

enum
{
    VOLUME,
    PITCH,
    VOICE,
    VOICESPEED,
    TEXTMODE,
    SETPOR,
    FREQ,
    TONE,
    REVERB
};


enum
{
    INIT,
    STRAIGHT,
    BRAKING_ZONE,
    CORNER_ENTRY,
    CORNER_MID,
    CORNER_EXIT,
};

enum
{
    UP,
    DOWN,
    OIL,
    TEMP,
    VOLTS,
    NO_SIGNAL,
    TPS,
    TPS_NOW,
    SPEED,
    SPLIT,
    LATERAL,
    LATERAL_GO,
	COAST,
	SPEED_SPLIT,
	NEW_LAP_LAPTIME,
    CURRENT_RPM,
    MINIMUM_SPEED,
    LINEAR,
    OVERREV,
    TIRELOCK,
    TIRELOCK_FRONT,
    TIRELOCK_REAR,
    GPS_ERROR,
    ENTRY_SPEED,
    TURNIN_SPEED,
    EXIT_SPEED,
    TPS_OFF_TO_ON,
    BEST,
    LAP_GAIN,
    MAX_LATERAL,
    MAX_LINEAR,
    NEW_LAP,
    GPS_VALID
           
};

enum
{
    RACEVOICE_CS_ORIGINAL,
    RACEVOICE_STANDALONE
};
#define DATA_LOG_FIFO 512
#define CAN_RX_THRESHOLD 50
#define OIL_NOT_MEASURED 0x1EAD
#define NUMBER_OF_CAN_IDS 8

struct _global_
{
    int can_trace;
	volatile int lapnumber;
    int tps;
    int engine_temp_f;
    int oil_pressure_psi;
    double lateral_g;
    double lateral_g_raw;
    double linear_g;
    double linear_g_raw;
    double zaxis_g;
    int mph;
    int brake_front_psi;
    int rpm;
    double delta_time;
    
    char timestring[64];
    double last_lap_time;
    volatile double running_lap_time;
    double last_delta_time;
	volatile double lap_time;
    
    int last_mph;
    long sys_timer;
    long rep_timer;
    int  rep_trigger;
    unsigned long canrx;
    unsigned long cantx;
    unsigned long cantx_fail;
    int can_irq;
    int tps_tickh;
    int tps_tickl;
    int latched_minspeed;
    unsigned long gps_rx_errors;
    double gps_lat;
    double gps_lng;
    unsigned long gps_distance;
    double volts;
    double linear_g_triggered;
    int overrev;
    int wheel_speed[4];
    int wheel_locked;
    double latched_max_lateral;
    double latched_max_linear;
    int latched_entry_speed;
    int latched_exit_speed;
    int latched_turn_in_speed;
    int throttle_timer;
    double tenths_timer;
    double throttle_timer_latched;
    int lap_complete;
    int split_reset;
    int rolling_lateral_g;
    
    double best_lap_time;
    int new_best_time;
    unsigned short phrase_triggers;
    int tire_pressure[6];
    int tire_temperature[6];
	unsigned int new_lap_started;
    unsigned char flash_id[3];
    unsigned char flash_erase;
    unsigned char flash_check;
    unsigned char flash_play;
    unsigned long flash_max_addr;
    unsigned long flash_rd_addr;
    unsigned long flash_wr_addr;
    unsigned long flash_last;
    unsigned long log_wr_idx;
    unsigned long log_rd_idx;
    int read_from_flash;
    int read_from_flash_step;
    int flash_record;
    int flash_init;
    int gps_error;
    int internal_log;
    int segment_mph;
    int board_version;
    int board_option;
    int gps_trace;
    int gps_update;
    int gps_sats;
    int gps_state;
    char gps_zda[GPS_ZDA_LEN];
    unsigned long gps_ticks;
    int accel_trace;
    int accel_poll;
    int no_connection;
    int custom_speech_trigger;
    int can_voice_speed;
    int can_voice_type;
    int can_voice_pitch;
    int can_sim;
    int dash_stream;
    int dash_send;
    int btlink;
    int btready;
    char bta[24];
    char btname[32];
    unsigned long hb;
    unsigned char custom_speech[CUSTOM_SPEECH_LEN+2];
    unsigned long CAN_IDS[NUMBER_OF_CAN_IDS];
    unsigned char ex_gforce;
    unsigned char ex_gps;
    unsigned long system_ready;
} ;



enum
{
        can_type_tps,
        can_type_rpm,
        can_type_oil_pressure,
        can_type_water_temperature,
        can_type_volts
};

struct _canconfig_ 
{
    unsigned long can_id;
    unsigned char resource;
    unsigned char mask[8];
    int type;
    int offset;
    float mult;
} ;



struct _settings_
{
    int trackindex;
    unsigned char trace_level;
    double volts;
    double lateral_g_high_trigger;
    double linear_g_high_trigger;
    int minmph_threshold;
    int mph;
    int mph_range[6];
    int rpm_overrev;
    int rpm_high;
    int rpm_low;
	int rpm_notice;
    int tps_high;
	int tps_low;
    int oil_low;
    int temp_high_f;
    unsigned long valid;
    int voice_speed;
    int lateral_gforce_announce;
    int linear_gforce_announce;
    int mph_announce;
    int overrev_announce;
    int upshift_announce;
    int downshift_announce;
    int temp_announce;
    int oil_announce;
    int volts_announce;
    int coach_mode;
    unsigned char can_trace;
    unsigned char dash_type;
    int segment_enable[MAX_SEGMENTS+2];
    double segment_start_lat[MAX_SEGMENTS+2];
    double segment_start_lng[MAX_SEGMENTS+2];
    double segment_stop_lat[MAX_SEGMENTS+2];
    double segment_stop_lng[MAX_SEGMENTS+2];
    int split_enable[MAX_SEGMENTS+2];
    double split_lat[MAX_SEGMENTS+2];
    double split_lng[MAX_SEGMENTS+2];
    char unit_name[UNIT_NAME_SIZE+2];
    int wheel_speed_brake_threshold;
    int wheel_speed_delta;
    int wheel_speed_enable;
    int voice_volume;
    int voice_type;
    int voice_pitch;
    int mph_trigger[MPH_TRIGGERS];
    int mph_trigger_delta;
    double corner_lateral_g_trigger;
    int lap_announce;
    int rpm_oil_threshold;
    int shift_tone;
    char phrase[MAX_PHRASE+2][PHRASE_LEN+2];
    int phrase_control[MAX_PHRASE+2];
    int gpswindow;
    int brake_tone_psi_low;
    int brake_tone_psi_high;
    int brake_tone_enable;
    int baud_rate;
    int brake_tone_hz;
    int brake_tone_duration;
    int shift_tone_hz;
    int shift_tone_duration;
    int tire_high_psi[6];
    int tire_low_psi[6];
    double checker_lattitude;
    double checker_longitude;
    char trackname[48];
    int log_enabled;
    int can_listen;
    int can_terminate;
    unsigned long gps_ticks;
    unsigned char dash_variant;
    unsigned char canconfig_name[PHRASE_LEN+2];
    struct _canconfig_  canconfig[NUMBER_OF_CAN_IDS];
};


#define SPEECH_RECORD 0x81
#define DATA_RECORD 0x55
#define LAP_RECORD 0x56
#define START_RECORD0 0xA0
#define START_RECORD1 0xA1
#define START_RECORD2 0xA2
#define START_RECORD3 0xA3
#define NO_RECORD 0xFF

#define SPEECH_LOG_SIZE_OVERRIDE 30
#define SPEECH_LOG_SIZE 2

#ifdef PCMODE
struct _data_ 
#else
struct __attribute__((__packed__))  _data_ 
#endif
{
#ifdef PCMODE
#pragma pack(push, 1)
#endif
    unsigned char record;
                                           // we use the override value here to write into the rest of the payload space
    unsigned char speech[SPEECH_LOG_SIZE]; // this should be here and pad the size for 32-byte frame in total
    float lateral;
    float linear;
    float gps_lat;
    float gps_lng;
    float lap_time;
    unsigned char gps_sats;
    short rpm;
    unsigned char mph;
    unsigned char tps;
    unsigned short sample;
    volatile unsigned short lapnumber;
#ifdef PCMODE
#pragma pack(pop)
#endif
} ;




#ifndef PCMODE
    // sstby on the first racevoice was on RG6
    #define SSTBYToggle() PLIB_PORTS_PinToggle(PORTS_ID_0, PORT_CHANNEL_G, PORTS_BIT_POS_6)
    #define SSTBYOn() PLIB_PORTS_PinSet(PORTS_ID_0, PORT_CHANNEL_G, PORTS_BIT_POS_6)
    #define SSTBYOff() PLIB_PORTS_PinClear(PORTS_ID_0, PORT_CHANNEL_G, PORTS_BIT_POS_6)
    #define SSTBYStateGet() PLIB_PORTS_PinGetLatched(PORTS_ID_0, PORT_CHANNEL_G, PORTS_BIT_POS_6)
    #define SSTBYStateSet(Value) PLIB_PORTS_PinWrite(PORTS_ID_0, PORT_CHANNEL_G, PORTS_BIT_POS_6, Value)

    #define SSTBY_NEWToggle() PLIB_PORTS_PinToggle(PORTS_ID_0, PORT_CHANNEL_D, PORTS_BIT_POS_0)
    #define SSTBY_NEWOn() PLIB_PORTS_PinSet(PORTS_ID_0, PORT_CHANNEL_D, PORTS_BIT_POS_0)
    #define SSTBY_NEWOff() PLIB_PORTS_PinClear(PORTS_ID_0, PORT_CHANNEL_D, PORTS_BIT_POS_0)
    #define SSTBY_NEWStateGet() PLIB_PORTS_PinGetLatched(PORTS_ID_0, PORT_CHANNEL_D, PORTS_BIT_POS_0)
    #define SSTBY_NEWStateSet(Value) PLIB_PORTS_PinWrite(PORTS_ID_0, PORT_CHANNEL_D, PORTS_BIT_POS_0, Value)
#endif
