/*----------------------------------------------------------------------

Lighting control for the Ideum touchless module.

Set PIXEL_COUNT to the total number of pixels to control.
Set NUMPIXELS_INTERNAL to the number of pixels to turn off in pedestal mode.
Set DEFAULT_COLOR to the color it should show at startup.

Usage: Send strings terminated with ; to the serial port:
"off;" - turns all pixels off with a wipe
"default;" - fades all pixels to default color
"#rrggbb;" - fades all pixels to RGB color in hex format
"pedestal;" - pedestal mode on, turns off first NUMPIXELS_INTERNAL pixels
"notpedestal;" - pedestal mode off
"brightness=###;" - sets the max brightness of the strip (0-255)
"time=###;" - approximate time in ms for color fade

-----------------------------------------------------------------------*/

#include <Adafruit_NeoPixel.h>
#ifdef __AVR__
 #include <avr/power.h> // Required for 16 MHz Adafruit Trinket
#endif

#define PIXEL_PIN    4  // Digital IO pin connected to the NeoPixels.
#define PIXEL_COUNT 128  // Total pixels
#define NUMPIXELS_INTERNAL 50 // Number of pixels inside the module
#define DEFAULT_COLOR 100,59,20 //r,g,b decimal - startup color

// Declare our NeoPixel strip object:
Adafruit_NeoPixel strip(PIXEL_COUNT, PIXEL_PIN, NEO_GRB + NEO_KHZ800);
// Argument 1 = Number of pixels in NeoPixel strip
// Argument 2 = Arduino pin number (most are valid)
// Argument 3 = Pixel type flags, add together as needed:
//   NEO_KHZ800  800 KHz bitstream (most NeoPixel products w/WS2812 LEDs)
//   NEO_KHZ400  400 KHz (classic 'v1' (not v2) FLORA pixels, WS2811 drivers)
//   NEO_GRB     Pixels are wired for GRB bitstream (most NeoPixel products)
//   NEO_RGB     Pixels are wired for RGB bitstream (v1 FLORA pixels, not v2)
//   NEO_RGBW    Pixels are wired for RGBW bitstream (NeoPixel RGBW products)

bool pedestal = false;
int fadeTime = 50; //ms
uint8_t maxBrightness = 255;

void setup() {
  Serial.begin(9600);
  strip.begin(); // Initialize NeoPixel strip object (REQUIRED)
  strip.show();  // Initialize all pixels to 'off'
  colorFade(strip.Color(DEFAULT_COLOR), 10, pedestal);
}

void loop() {
  // check for incoming serial data:
  while (Serial.available() > 0) {
    // read incoming serial data:
    String inString = Serial.readStringUntil(';');
    if (inString.startsWith("#")) 
    {
      // Reads hex color such as #ff0000 for 100% red
      long long number = strtoll( &inString.begin()[1], NULL, 16);
      // Split them up into r, g, b values
      long long r = number >> 16;
      Serial.printf("%d,",r);
      long long g = number >> 8 & 0xFF;
      Serial.printf("%d,",g);
      long long b = number & 0xFF;
      Serial.printf("%d\n",b);
      colorFade(strip.Color(r,g,b),fadeTime,pedestal);
    }
    else if (inString.startsWith("pedestal")) {
        pedestal = true;
        Serial.println("Pedestal mode on");
        for(int i=0; i<NUMPIXELS_INTERNAL; i++) 
        {
          strip.setPixelColor(i, strip.Color(0,0,0));
          strip.show();
        }
    }
    else if (inString.startsWith("notpedestal")) {
        pedestal = false;
        Serial.println("Pedestal mode off");
        colorFade(strip.Color(DEFAULT_COLOR), fadeTime, pedestal);
    }
    else if (inString.startsWith("off")) {
        Serial.println("Off");
        colorWipe(strip.Color(0,0,0), 1, false);    // all off
    }
    else if (inString.startsWith("default")) {
        Serial.println("Default color");
        maxBrightness = 255;
        colorFade(strip.Color(DEFAULT_COLOR), fadeTime, pedestal);
    }
    else if (inString.startsWith("brightness"))
    {
      String s = inString.substring(11);
      maxBrightness = s.toInt();
      strip.setBrightness(maxBrightness);
      strip.show();
      Serial.println("Brightness: "+ s);
    }
    else if (inString.startsWith("time"))
    {
      String s = inString.substring(5);
      fadeTime = s.toInt();
      Serial.println("Time: "+ s +"ms");
    }
    else {
      Serial.println("No comprendo");
    }
  }
}

// Fades all pixels from black to the specified color 
void colorFade(uint32_t color, int time, bool pedestal) 
{
  if (maxBrightness == 0) {
    time = 0;
    color = strip.Color(0,0,0);
  }
  if (time == 0) {
    if (pedestal) {
      //Blank out the internal LEDs if it's on the pedestal
      for(int i=0; i<strip.numPixels(); i++) 
      {
        if (i<NUMPIXELS_INTERNAL) strip.setPixelColor(i, strip.Color(0,0,0));
        else strip.setPixelColor(i, color);
      }
    }
    else {
        for(int i=0; i<strip.numPixels(); i++) 
        {
          strip.setPixelColor(i, color);
        }
    }
    strip.setBrightness(maxBrightness);
    strip.show();               
  }
  else {
    int t = time / maxBrightness;
    int increment = 1;
    while (t<1) {
      increment++;
      t = time * increment / maxBrightness;
    }
    for (int fadeValue = 0 ; fadeValue <= maxBrightness; fadeValue += increment) 
    {
      if (pedestal) {
        //Blank out the internal LEDs if it's on the pedestal
        for(int i=0; i<strip.numPixels(); i++) 
        {
          if (i<NUMPIXELS_INTERNAL) strip.setPixelColor(i, strip.Color(0,0,0));
          else strip.setPixelColor(i, color);
        }
      }
      else {
          for(int i=0; i<strip.numPixels(); i++) 
          {
            strip.setPixelColor(i, color);
          }
      }
      strip.setBrightness(fadeValue);
      strip.show();                  
      delay(t);
    }
  }
}

// Fill strip pixels one after another with a color. Strip is NOT cleared
// first; anything there will be covered pixel by pixel. Pass in color
// (as a single 'packed' 32-bit value, which you can get by calling
// strip.Color(red, green, blue) as shown in the loop() function above),
// and a delay time (in milliseconds) between pixels.
void colorWipe(uint32_t color, int wait, bool pedestal) 
{
  for(int i=0; i<strip.numPixels(); i++) 
  { 
    if (pedestal && i<NUMPIXELS_INTERNAL) strip.setPixelColor(i, 0);
    else  strip.setPixelColor(i, color);         
    strip.show();                          
    delay(wait);                           
  }
}
