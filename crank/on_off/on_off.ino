#define ACK "ACK\n"

void setup() {
  Serial.begin(9600);
  while (true) {
    Serial.print("Crank init begin\n");
    delay(10);
    if (Serial.readStringUntil('\n') == "Conrod Received")
      break;
  }
  Serial.print(ACK);
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, HIGH);
  delay(2000);
  digitalWrite(LED_BUILTIN, LOW);
}

void loop() {
  checkSerialInput();
  updateState();
  sendState();
  delay(10);
}


void sendState() {
  
}


void updateState() {
  
}

void checkSerialInput() {
  if (Serial.available()) {
    String readString = Serial.readString();
    // Do stuff with the read string (process command etc)
  }
}
