// -------------------------------------------------
// Gay Nation Vendors
// -------------------------------------------------
// This is a sample vendor script to use the StarDust Currency System
// Below are some variables you need to change.
// The Server and vendors must be on the same region for now

// all vendors & Servers should have the same values on these variables
string yourSecurePassword = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";     // <-- Change this
integer securePin = 666;                                                 // <-- Change this
integer yourchannel = 456781;                                            // <-- Change this

// If you are using HTTP or Email this must be filled out
string serverKey = "00000000-0000-0000-0000-000000000000"; // <-- Change this to the value your server says
string serverHTTP = "http://127.0.0.1:9000/lslhttp/00000000-0000-0000-0000-000000000000/"; // <-- Change this to the value your server says

// here you set the comms type. 
// You can use
// LISTEN, HTTP, EMAIL
// All three methods work, but! email does not work from region to region yet.
// Because of this HTTP does not work between regions as well 
// (because the web url has to be emailed to the vendor)
// Only change this if you know what you are doing
string commsType = "LISTEN";

// -------------------------------------------------
// these values will change for each vendor
// -------------------------------------------------
// this filter lets you filter what items are returned from the server by the catagory
string productCatFilter = "";

// -------------------------------------------------
// do not change anything below here
// -------------------------------------------------
string internalemaildomain = "@lsl.gaynation.net";
list items = [];
integer listener = 0;
integer menulistener = 0;
integer menuchannel = 0;
list touchmenu;
list adminmenu;

integer itempos = 0;
list item;
key notexture = "dfaeba1d-bec0-4661-9878-d421874d541c";
integer catagory = 0;
integer uniqueId = 1;
integer objectName = 2;
integer texture = 3;
integer sound = 4;
integer web = 5;
integer video = 6;
integer note = 7;
integer price = 8;
integer product = 9;
integer description = 10;

integer next = -2;
integer prev = -2;

integer panelCount = 0;
integer waitingForItems = 0;
// panels are not used yet.. still need to finish it
list panels = [0,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2,-2];


DoRequest(key id, string method, string message, string commtype)
{
    list command = llParseStringKeepNulls(message, ["|"], []);
    string whatAmIDoing = llList2String(command, 1);
    string response = "";
    if (llList2String(command, 0) == llMD5String(yourSecurePassword + llGetDate(), securePin + dayOfYear()))
    {
        if (whatAmIDoing == "ITEMS")
        {
            if (llList2String(command, 2) == (string)llGetKey())
            {
                items = llList2List(command, 3, -1);
                DisplayItem(0, 0);
                waitingForItems = 0;
            }
        }
        else if (whatAmIDoing == "Web")
        {
            llLoadURL(llDetectedKey(0), llList2CSV(llList2List(item, description, -1)), llList2String(item, web));
        }
        else if (whatAmIDoing == "Video")
        {
            llLoadURL(llDetectedKey(0), llList2CSV(llList2List(item, description, -1)), llList2String(item, video));
        }
        else if (whatAmIDoing == "Notecard")
        {
            Send2Server("GIVEITEM|" + llList2String(item, note) + "|" + (string)id);
        }
        else if (whatAmIDoing == "Buy")
        {
            llSay(0, "To purchase just right click the vendor and pay G$" + llList2String(item, price));
        }
        else if (llGetOwnerKey(id) == llGetOwner())
        {
            if (whatAmIDoing == "READY")
            {
                if (waitingForItems)
                {
                    Send2Server("GETITEMS|" + (string)llGetKey() + "|" + productCatFilter);
                }
            }
            else if (whatAmIDoing == "Reset")
            {
                llResetScript();
            }
        }
    }
}

Send2Server(string message)
{
    if (commsType == "LISTEN")
    {
        llRegionSay(yourchannel, llMD5String(yourSecurePassword + llGetDate(), securePin + dayOfYear()) + "|" + message);
    }
    else if (commsType == "EMAIL")
    {
        llEmail(serverKey + internalemaildomain, "", llMD5String(yourSecurePassword + llGetDate(), securePin + dayOfYear()) + "|" + message );
    }
    else if (commsType == "HTTP")
    {
        llHTTPRequest(serverHTTP + "?DATA=" + llMD5String(yourSecurePassword + llGetDate(), securePin + dayOfYear()) + "|" + message, [], "" );
    }
}


integer DisplayItem(integer pos, integer panel)
{
    // Catagory, Uinque ID, Object Name, Texture Key, Sound Key, product page, product video page, Note Card Name, Price, product name, description
    if (pos <= -1)
        pos = llGetListLength(items) - 1;
    if (pos >= llGetListLength(items))
        pos = 0;
    
    
    list item2 = llParseStringKeepNulls(llList2String(items, pos), [","], []);
    
    if (menuchannel == 0) menuchannel = randIntBetween(-99999999, 99999999);
    if (menulistener == 0) menulistener = llListen(menuchannel, "", "", "");
    
    key texture2Use = notexture;
    if (llStringLength(llList2String(item2,texture)) == 36) texture2Use = llList2Key(item2, texture);
    llSetLinkPrimitiveParamsFast(llList2Integer(panels,panel), [PRIM_TEXTURE, 0, texture2Use, <1,1,0>, ZERO_VECTOR , 0.0 ]);
    
    if (panel == 0)
    {
        item = item2;
        llSetPayPrice(llList2Integer(item, price), [llList2Integer(item, price) , PAY_HIDE , PAY_HIDE , PAY_HIDE]);
        if (llStringLength(llList2String(item,sound)) == 36) llPlaySound(llList2Key(item, texture), 1.0);
        touchmenu = ["Buy"];
        if (llList2String(item, web) != "")
            touchmenu += ["Web"];
        if (llList2String(item, video) != "")
            touchmenu += ["Video"];
        if (llList2String(item, note) != "")
            touchmenu += ["Notecard"];        
        adminmenu = ["Reset"] + touchmenu;
        llSay(0, llList2CSV(llList2List(item, description, -1)));
    }
    
    return pos;
}

integer dayOfYear()
{
    list dateComponents = llParseString2List(llGetDate(), ["-"], []);
    integer year = (integer) llList2String(dateComponents, 0);
    integer month = (integer) llList2String(dateComponents, 1);
    integer day = (integer) llList2String(dateComponents, 2);
    return
          day
        + (month - 1) * 30 - (2 - ((year % 4) == 0) * ((year % 100) != 0)) * (month > 2)
        + (month / 2) * (month <= 8) + (4 + (month - 7) / 2) * (month > 8);
}
 
integer randIntBetween(integer min, integer max)
{
    return min + randInt(max - min);
}

integer randInt(integer n)
{
     return (integer)llFrand(n + 1);
}

FindNextPrev()
{
    integer primcount = llGetNumberOfPrims( );
    integer i = -1;
    panelCount = 0;
    while(primcount >= ++i)
    {    
        string primname = llGetLinkName(i);
        if (primname == "Next")
            next = i;
        else if (primname == "Prev")
            prev = i;
        else if (llGetSubString(primname,0,4) == "Panel")
        {
            integer panelNum = (integer)llGetSubString(primname,5,-1);
            panels = llListReplaceList(panels, [i], panelNum, panelNum);
            panelCount++;
        }
    }
    
}

default
{
    state_entry()
    {
        FindNextPrev();
        llRequestPermissions(llGetOwner(), PERMISSION_DEBIT);
    }
    
    run_time_permissions(integer perm)
    {
        if (perm & PERMISSION_DEBIT)
        {
            if (listener > 0) llListenRemove(listener);
            listener = 0;
            if (commsType == "LISTEN")
            {
                listener = llListen(yourchannel, "", "", "");
            }
            else if (commsType == "EMAIL")
            {
                llSetTimerEvent(5);
            }
            waitingForItems = 1;
            Send2Server("GETITEMS|" + (string)llGetKey() + "|" + productCatFilter);
        }
        else
        {
            llSay(0, "This is a vendor, and can not function without permissions to use money.");
            llRequestPermissions(llGetOwner(), PERMISSION_DEBIT);
        }
    }
    
    listen( integer channel, string name, key id, string message )
    {
        if (channel == menuchannel)
            message = llMD5String(yourSecurePassword + llGetDate(), securePin + dayOfYear()) + "|" + message;
        if ((channel != menuchannel) && (llGetOwnerKey(id) != llGetOwner())) return;
        DoRequest(id, "", message, "LISTEN");
    }

    email( string time, string address, string subject, string message, integer num_left )
    {
        message = llDeleteSubString(message, 0, llSubStringIndex(message, "\n\n") + 1);
        DoRequest((key)llGetSubString(address, 0, 35), "", message, "EMAIL");
        if(num_left)
            llGetNextEmail("", "");
    }

    timer()
    {
        llGetNextEmail("", "");
    }
    
    touch_start(integer a)
    {
        if (llDetectedLinkNumber(0) == next)
            itempos = DisplayItem(itempos + 1, 0);
        else if (llDetectedLinkNumber(0) == prev)
            itempos = DisplayItem(itempos - 1, 0);
        else if (menuchannel != 0)
        {
            if (llDetectedKey(0) == llGetOwner())
                llDialog(llDetectedKey(0), llList2CSV(llList2List(item, description, -1)), adminmenu, menuchannel );
            else
                llDialog(llDetectedKey(0), llList2CSV(llList2List(item, description, -1)), touchmenu, menuchannel );
        }
    }
    
    http_response(key request_id, integer status, list metadata, string body)
    {
        DoRequest(serverKey, "", body, "HTTP");
    }
    
    money(key id, integer amount)
    {
        if(amount != llList2Integer(item, price))
        {
            llGiveMoney(id, amount);
            llInstantMessage(id, "You paid "+(string)amount+", which is the wrong price, the price is: "+(string)llList2Integer(item, price));
        }
        else
        {
            Send2Server("GIVEITEM|" + llList2String(item, objectName) + "|" + (string)id);
        }
    }
    
    changed(integer What) 
    {
        if ((What & CHANGED_REGION_START) || (What & CHANGED_REGION_RESTART) || (What & CHANGED_REGION))
        {
            waitingForItems = 1;
            Send2Server("GETITEMS|" + (string)llGetKey() + "|" + productCatFilter);
        }
    }
}