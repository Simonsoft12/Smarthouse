#include <SPI.h>
#include <Ethernet.h>
#include <Bridge.h>
#include <HttpClient.h>

const int arduinoID = 91; //Arduino1
//this is for ethernet
byte mac[] = { 0xDE, 0xAD, 0xCE, 0xEF, 0xFE, 0xED };
byte ip[] = { 192, 168, 1, arduinoID }; // salon
byte gateway[] = { 192, 168, 1, 1 };
byte subnet[] = { 255, 255, 255, 0 };
EthernetServer server(80);
String readString;
//This is Main server that recievies the updates
char mainServerIP[] = "192.168.1.200"; // used in this for sending updates of actions
EthernetClient arduinoClient;

//for how long button needs to be pressed to switch light to auto ( pir)
int buttonDelayTime = 4000;        //4 sec
// PIR SENSOR
// Define the *minimum* length of time, in milli-seconds, after witch will switch on the PIR sensor
unsigned long pirDelay = 60000;    // time for how long stays on  (       b300000 ms = 10 minutes)
unsigned long lastTimePir;

//Salon
const byte buttonSalonGPin = 2;     // główne światło przycisk
const byte buttonSalonJPin = 3;     // jadalnia światło przycisk
const byte buttonSalonPsofaPin = 5; // punktowe sofa światło przycisk

const byte relaySalonJPin = 6;      // jadalnia światło przekaźnik
const byte relaySalonGPin = 8;      // głowne światło przekaźnik
const byte relaySalonPsofaPin = 23; // punktowe sofa światło przekaźnik

const byte reedSalonPin = 24;       // czujnik Otwarcia Okna - kontrakton

byte buttonSalonGPushCounter = 0;   // głowne dzwonkowy
byte buttonSalonJState = 1;
byte buttonSalonPsofaState = 1; 

byte windowSalonState ;             // okno



//  IN - czyli parametry przesyłane do manipulacji przełącznikami
String switchesParms[][2] = {
  {"SGon", "SGoff"},     //Salon Glowne      		1
  {"SJon", "SJoff"},     //Salon Jadalnia   		2
  {"SPsofaon", "SPsofaoff"}, // Salon Sofa  		4
} ;

//                                1             2                    4                   20     
byte switchesRelayPins[] = {relaySalonGPin, relaySalonJPin , relaySalonPsofaPin ,   reedSalonPin,  };
// OUT
String statusOut[] = {      "SalonG",        "SalonJ",          "SalonPsofa",         "SalonO",    };

//********************************************************************************************************************************************************
//********************************************************************************************************************************************************

// ----------------------------------------------------------------- SETUP --------------------------------------------------------------
void setup()
{
  // initialize serial communication - for serial monitor
  Serial.begin(9600);
  //Activate Ethernet
  Ethernet.begin(mac, ip, gateway, subnet);
  // Ethernet.begin(mac);
  server.begin();
  Serial.print("arduino IP is ");
  Serial.println(Ethernet.localIP());

  //SALON
  // initialize the button pin as a input:
  pinMode(buttonSalonGPin, INPUT_PULLUP);
  pinMode(buttonSalonJPin, INPUT_PULLUP);
  pinMode(buttonSalonPsofaPin, INPUT_PULLUP);
  // initialize the relay pin as an output:
  pinMode(relaySalonGPin, OUTPUT);
  pinMode(relaySalonJPin, OUTPUT);
  pinMode(relaySalonPsofaPin, OUTPUT);
  pinMode(reedSalonPin, INPUT_PULLUP); // one pin goes to PIN_number, other to Ground
  buttonSalonJState = digitalRead(buttonSalonJPin);
  buttonSalonPsofaState = digitalRead(buttonSalonPsofaPin);
}
// ---------------------------------------------------------- END OF SETUP --------------------------------------------------------------

//********************************************************************************************************************************************************
//********************************************************************************************************************************************************

//------------------------------------------------------------- LOOP --------------------------------------------------------------------
void loop()
{
  // SALON
  buttonSalonGPushCounter = Button2Relay(buttonSalonGPin, buttonSalonGPushCounter, buttonDelayTime, relaySalonGPin);   // główne dzwonkowy
  buttonSalonJState = ButtonStatusFunction(buttonSalonJState, buttonSalonJPin, relaySalonJPin) ;
  buttonSalonPsofaState = ButtonStatusFunction(buttonSalonPsofaState, buttonSalonPsofaPin, relaySalonPsofaPin) ;
  windowSalonState = WindowStatusFunction(windowSalonState, reedSalonPin, "Salon"); // okno Salon

  //********************************************************************************************************************************************************
  //********************************************************************************************************************************************************

  //--------------------------- CALL FROM Main SERVER -------------------------------------------------
  // This is only going through if the request from the home server is sent
  // Create a client connection
  EthernetClient client = server.available();
  if (client)
  {
    while (client.connected())
    {
      if (client.available())
      { //read HTTP request char by char until its end \n
        char c = client.read();
        if (readString.length() < 100)
        {
          readString += c;
        }

        if (c == '\n')
        {
          for (int i = 0; i < (sizeof(switchesParms) / sizeof(switchesParms[0][0])) ; i++)
          {
            if (readString.indexOf(switchesParms[i][0]) > 0)
            {
              if (switchesParms[i][0] == "SGon") {
                buttonSalonGPushCounter = 1;
                Serial.println("Salon Glowne button push on");
              }
              else
                digitalWrite(switchesRelayPins[i], HIGH);
            }
            if (readString.indexOf(switchesParms[i][1]) > 0)
            {
              if (switchesParms[i][1] == "SGoff")
              {
                buttonSalonGPushCounter = 2;
                Serial.println("Salon Glowne button push off");
              }
              else
                digitalWrite(switchesRelayPins[i], LOW);
            }
          }

          //RESPONSE
          client.println("HTTP/1.1 200 OK"); //send new page
          client.println("Content-Type: text/html");
          client.println();
          //for (int i = 0; i < 26 ; i++)
          for (int i = 0; i < (sizeof(statusOut) / sizeof(statusOut[0])); i++)  
          {
            // tutaj zwracana jest nazwa wskaźnika
            String str = statusOut[i];
            client.print(str);
            client.print("=");

            if (str.endsWith("T"))
            {
              String str2 = CutLastChar(str);
              if (str2.endsWith("T")) client.print(Psychrometr(switchesRelayPins[i], "T"));
              else if (str2.endsWith("H")) client.print(Psychrometr(switchesRelayPins[i], "H"));
              else client.print(Thermometer(switchesRelayPins[i]));
             }
            else if (str == "SalonG")
            {
              if (buttonSalonGPushCounter == 1)  client.print("1");
              else if (buttonSalonGPushCounter == 2)  client.print("0");
            }
            else
              client.print(digitalRead(switchesRelayPins[i]));
              
            client.println(";");
          }
          readString = "";
          delay(10);
          client.stop();
        }
      }
    } // zamknięcie while  //closing while
  }
  //--------------------------- Endo of CALL FROM MAIN SERVER ----------------------------------------------------
}
//------------------------------------------------------------- END OF LOOP --------------------------------------------------------------------

//********************************************************************************************************************************************************
//********************************************************************************************************************************************************

//------------------------------------------------------------- FUNCTIONS ------------------------------------------------------------------

// ------------------------ FUNCTION FOR SENDIONG HTTP REQUESTS TO MAIN SERVER ---------------------------------
// this is to send request to the server with windows state
void HttpRequest(byte arduinoID, int objectID, int actionID, int stateID) {
  if (arduinoClient.connect(mainServerIP, 8080)) {
    Serial.println("Sending request to JBOSS server");
    // Make a HTTP request:
    arduinoClient.print( "GET /Smartupdate/update/");
    arduinoClient.print( arduinoID );
    arduinoClient.print(  "/" );
    arduinoClient.print( objectID );
    arduinoClient.print(  "/" );
    arduinoClient.print( actionID );
    arduinoClient.print(  "/" );
    arduinoClient.print( stateID );
    arduinoClient.println( " HTTP/1.1");
    arduinoClient.print( "Host: " );
    arduinoClient.println(mainServerIP);
    arduinoClient.println();
    arduinoClient.stop();
  }
  else {
    // if you couldn't make a connection:
    Serial.println("connection failed");
  }
}
//---------------------------------- END OF SENDIONG HTTP REQUESTS ---------------------------------------------------


//---------------------------------- Function for detecting button change ---------------------------------------------
// This is to check BUTTON STATE - updates the server only when button possition has been changed
byte ButtonStatusFunction(byte lastButtonState, byte buttonPinNumber, byte relayPinNumber)
{
  byte buttonState = digitalRead(buttonPinNumber);
  byte relayState = digitalRead(relayPinNumber);
  if (buttonState != lastButtonState)
  {
    //change the relay state to opposit
    if (relayState) {
      digitalWrite(relayPinNumber, LOW);
      Serial.print("Light turned off by the button: ");
      Serial.println(buttonPinNumber);
    }
    else {
      digitalWrite(relayPinNumber, HIGH);
      Serial.print("Light turned on by the button: ");
      Serial.println(buttonPinNumber);
    }
  }
  return lastButtonState = buttonState;
}
//---------------------------------- END OF BUTTON STATE UPDATE ------------------------------------------------------


//------------------- Function for WINDOW STATE UPDATE ---------------------------------------------------------------
// This is to check WINDOWS STATE - updates the server only when window is being opened or closed
byte WindowStatusFunction(byte lastWindowState, byte windowPinNumber, String windowName)
{
  byte windowState = digitalRead(windowPinNumber);

  if (windowState != lastWindowState)
  {
    if (windowState == HIGH)
    {
      Serial.print(windowName);
      Serial.println(" window is opened");
      //HttpRequest(arduinoID, 1, 1, windowState);
      lastWindowState = windowState;
      return 1;
    }
    else
    {
      Serial.println("updating state to: false - window is closed");
      //HttpRequest(arduinoID, 1, 1, windowState);
      lastWindowState = windowState;
      return 0;
    }
    delay(50);
  }

  return  windowState ;
}
//---------------------------------- end of WINDOW STATE UPDATE ------------------------------------------------------


// -------------- Function for light switching - checks how long was the button pressed  ---------------------------------
// that part of code will block execution of rest of the code until the button is released
boolean ButttonPressCheck(byte buttonPin, int delayValue) {  // delayValue jako typ int dlatego, że 30 sekund jako maksymalna wartość to wystarczy.
  float pressLength_milliSeconds = 0;
  //Record how long the button in being held down
  while (digitalRead(buttonPin) == LOW ) {
    delay(50);  //if you want more resolution, lower this number
    pressLength_milliSeconds = pressLength_milliSeconds + 100;
    //display how long button is has been held
    Serial.print("ms = ");
    Serial.println(pressLength_milliSeconds);
  }
  if (pressLength_milliSeconds >= delayValue) {
    pressLength_milliSeconds = 0;
    return true;
  }
  else {
    pressLength_milliSeconds = 0;
    return false;
  }
}
// -------------- END OF Function for light switching -------------------------------------------------------------------


// ------------------------------- This is for light switching dzwonkowy ------------------------------------------------
byte Button2Relay(byte buttonPin, byte buttonPushCounter, int buttonDelay, byte relayPin) {
  byte buttonState = digitalRead(buttonPin);
  // if the state has changed, increment the counter
  if (buttonState == LOW) {
    // if the current state is HIGH then the button
    // went from off to on:
    buttonPushCounter++;
    Serial.print("Salon główne  button number: ");
    Serial.println(buttonPin);
    Serial.print("Salon główne number of pushes: ");
    Serial.println(buttonPushCounter);
    if (ButttonPressCheck(buttonPin, buttonDelay ))
    {
      buttonPushCounter = 3;
    }
  }
  // Delay a little bit to avoid bouncing
  delay(50);

  //turns light on/off , 1st push - on ; 2nd push - Off
  if (buttonPushCounter == 1) {
    digitalWrite(relayPin, HIGH);
    Serial.println("Salon główne  relay ON");
  }
  else if (buttonPushCounter == 2)
  {
    // reset the button counter - only in the last action
    digitalWrite(relayPin, LOW);
    Serial.println("Salon główne relay OFF");
    buttonPushCounter = 0;
  }
  else if (buttonPushCounter == 3)
  {
    Serial.println("Salon główne buttonPushCounter = 1");
    buttonPushCounter = 1;
  }
  return buttonPushCounter;
} 
// ------------------------------- END of Button2Relay --------------------------------------------------------------------------------


// ------------------------------- this is for light switching on/off with pir support ------------------------------------------------
byte Button2RelayPir(byte buttonPin, byte buttonPushCounter, int buttonDelay, byte relayPin, byte pirPin, unsigned long pirDelay) {
  byte buttonState = digitalRead(buttonPin);
  // if the state has changed, increment the counter
  if (buttonState == LOW) {
    // if the current state is HIGH then the button
    // went from off to on:
    buttonPushCounter++;
    Serial.print("number of button pushes:  ");
    Serial.println(buttonPushCounter);
    if (ButttonPressCheck(buttonPin, buttonDelay ))
    {
      buttonPushCounter = 3;
      Serial.println(buttonPushCounter);
    }
  }
  // Delay a little bit to avoid bouncing
  delay(50);

  //turns light on/off , 1st push - on ; 2nd push - Off
  if (buttonPushCounter == 1) {
    digitalWrite(relayPin, HIGH);
    //Serial.print("relay ON");
  }
  else if (buttonPushCounter == 2)
  {
    // reset the button counter - only in the last action
    digitalWrite(relayPin, LOW);
    //Serial.println("relay OFF");
    buttonPushCounter = 0;
  }
  else if (buttonPushCounter == 3) {
    // execute pir sensor
    Pir2Relay(relayPin, pirPin, pirDelay);

  } else if (buttonPushCounter == 4) {
    buttonPushCounter = 1;
  }
  return buttonPushCounter;
} 
// ------------------------------- END of Button2RelayPir -----------------------------------


// ------------------------------------ PIR SENSOR ------------------------------------------
void Pir2Relay(byte relayPin, byte pirPin, unsigned long pirDelay) {
  //  Serial.println("3 clicks ");
  unsigned long currentMillis = millis();

  if (currentMillis - lastTimePir < pirDelay) {
    digitalWrite(relayPin, HIGH);
    Serial.println("pir2relay On");
  } else {
    digitalWrite(relayPin, LOW);
    Serial.println("pir2relay Off");
  }

  if ( digitalRead(pirPin) == HIGH) {
    lastTimePir = currentMillis;
    Serial.println("PIR HIGH");
  }
  Serial.println(lastTimePir);

}
//-----------------------------------------END--OF--PIR--SENSOR-----------------------------------


//----------------------------------------- Function for Cutting last char -----------------------------------
String CutLastChar(String string)
{
  // normalnie liczyłoby wszystkie znaki + 1 jako znak konca, więc nie dodaje 
  //znaku końca i tym samym ostatnia litera jest znakiem końca 
  //i automatycznie zostaje obcięta.
  // Length (with one extra character for the null terminator)
  int str_len = string.length(); //+ 1;
  
  // Prepare the character array (the buffer) 
  char char_array[str_len];
  
  // Copy it over to char_array
  string.toCharArray(char_array, str_len);
  
  String string2(char_array);

  return string2;
}

//------------------------------------------------------- END OF FUNCTIONS --------------------------------------------------------------
