// -------------------------------------------------
// Gay Nation Server for Vendor
// -------------------------------------------------
// This is a sample Sever script to use on Gay Nation Grid
// Below are some variables you need to change.
// The Server and vendors must be on the same region for now

// all vendors & Servers should have the same values on these variables
string yourSecurePassword = "QkzG3mAjuzwvE4VKY3tGZb38C6czm9V23XgT";        // <-- Change this
integer securePin = 666;                                                // <-- Change this
integer yourchannel = 456781;                                            // <-- Change this

// -------------------------------------------------
// Change nothing below here
// -------------------------------------------------
string internalemaildomain = "@lsl.gaynation.net";
string url;
list items = [];
integer nc_line = 0;
list report;
key cqid;
integer listener = 0;
integer loadingNotecard = 0;
integer finishedLoading = 0;

string DoRequest(key id, string method, string body, string type)
{
    // llOwnerSay(body);
    list command = llParseStringKeepNulls(body, ["|"], []);
    string whatAmIDoing = llList2String(command, 1);
    string response = "";
    if (llList2String(command, 0) == llMD5String(yourSecurePassword + llGetDate(), securePin + dayOfYear()))
    {
        list items2Return = items;
        if (whatAmIDoing == "GETITEMS")
        {
            if (llGetListLength(command) >= 3)
            {
                if (llList2String(command, 3) != "")
                    items2Return = SearchForItems(llList2String(command, 3), 0);
            }
            response = "ITEMS|" + llList2String(command, 2) + "|" + llDumpList2String(items2Return, "|" );
        }
        else if (whatAmIDoing == "GIVEITEM")
        {
            if (llGetInventoryType(llList2String(command, 2)) != INVENTORY_NONE)
            {
                llGiveInventory((key)llList2String(command, 3), llList2String(command, 2));
            }
            else
            {
                items2Return = SearchForItems(llList2String(command, 2), 1);
                if (llGetListLength(items2Return) == 1)
                {
                    response = "SENT";
                    llGiveInventory((key)llList2String(command, 3), llList2String(items2Return, 2));
                    report += ["Item " + llList2String(items2Return, 1) + " sent to " + llList2String(command, 4) ];
                }
                else
                    response = "ITEMNOTFOUND";
            }
        }
        if (type == "HTTP")
            llHTTPResponse(id, 200,  llMD5String(yourSecurePassword + llGetDate(), securePin + dayOfYear()) + "|" + response);
        else if (type == "LISTEN")
            llRegionSay(yourchannel, llMD5String(yourSecurePassword + llGetDate(), securePin + dayOfYear()) + "|" + response);
        else if (type == "EMAIL")
        {
            llEmail((string)id + internalemaildomain, "", llMD5String(yourSecurePassword + llGetDate(), securePin + dayOfYear()) + "|" + response );
        }
    }
}

list SearchForItems(string filter, integer pos)
{
    list items2Return;
    integer itemCount = llGetListLength(items);
    integer i = -1;
    while(itemCount > ++i)
    {
        if (llStringTrim(llToLower(llList2String(llParseStringKeepNulls(llList2String(items, i), [","], []), pos)), STRING_TRIM) == llStringTrim(llToLower(filter), STRING_TRIM))
            items2Return += llList2List(items, i, i);
    }
    return items2Return;
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

LoadNoteCard()
{
    if (llGetInventoryType("~PRODUCTS") == INVENTORY_NOTECARD)
    {
        loadingNotecard = 0;
        llSetTimerEvent(0);
        items = [];
        nc_line = 0;
        llSay(0,"[LOADING] Loading Products");
        cqid = llGetNotecardLine("~PRODUCTS", nc_line);
    }
    else
    {
        loadingNotecard = 1;
        llSetTimerEvent(10);
    }
}

SayDetails()
{
    llSay(0,"[INFO] URL = " + url);
    llSay(0,"[INFO] KEY = " + (string)llGetKey());
}
 
default 
{
    state_entry() 
    {
        finishedLoading = 0;
        LoadNoteCard();
    }
    
    touch_start(integer a)
    {
        SayDetails();
    }
    
    changed(integer What) 
    {
        if ((What & CHANGED_REGION_START) || (What & CHANGED_REGION_RESTART) || (What & CHANGED_REGION))
        {
            llSetColor(<1,0,0>,0);
            llResetScript();
        }
    }
    
    http_request(key id, string method, string body) {
        if (method == URL_REQUEST_GRANTED) {
            //Saying URL to owner
            url = body;
            llSay(0,"[LOADING] Finished");
            SayDetails();
            llSetColor(<0,1,0>,0);
            if (!finishedLoading) llRegionSay(yourchannel, llMD5String(yourSecurePassword + llGetDate(), securePin + dayOfYear()) + "READY");
            finishedLoading = 1;
        } else if (method == URL_REQUEST_DENIED)
            llOwnerSay("[ERROR] No URLs free !");
        else if (method == "GET") 
            DoRequest(id, method, llGetSubString(llGetHTTPHeader(id,"x-query-string"), 5, -1), "HTTP");
    }
    
    dataserver(key queryid, string data)
    {
        if ((cqid == queryid) && (data != EOF)) {    
            if (llStringTrim(data, STRING_TRIM) != "")
            {
                if (llStringTrim(llGetSubString(data, 0, 2), STRING_TRIM) != "//")
                {
                    list temp = llParseStringKeepNulls(data, [","], []);
                    integer tempc = llGetListLength(temp);
                    integer ii = -1;
                    data = "";
                    while(tempc > ++ii)
                    {
                        data += llStringTrim(llList2String(temp,ii), STRING_TRIM) + ",";
                    }
                    items += [llGetSubString(data,0, llStringLength(data) - 2 )];
                }
            }
            nc_line++;
            cqid = llGetNotecardLine("~PRODUCTS", nc_line);
        } else if ((cqid == queryid) && (data == EOF)) {
            llSay(0,"[LOADING] " + (string)llGetListLength(items) + " products loaded.");
            
            llSay(0,"[LOADING] Starting Listener");
            if (listener > 0) llListenRemove(listener);
            listener = llListen(yourchannel, "", "", "");
            
            llSay(0,"[LOADING] Starting Email Checker");
            loadingNotecard = 0;
            llSetTimerEvent(0.5);
            
            llSay(0,"[LOADING] Loading URL");
            llRequestURL();
        }
    } 
    
    listen( integer channel, string name, key id, string message )
    {
        if (llGetOwnerKey(id) != llGetOwner()) return;
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
        if (loadingNotecard) LoadNoteCard();
        else llGetNextEmail("", "");
    }
}