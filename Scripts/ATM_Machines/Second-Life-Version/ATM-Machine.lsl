string notifyEmail = "YOUREMAIL@GOES.HERE";
string ThisGridName = "Second Life";
string OtherGridName = "YOUR GRID NAME GOES HERE";
string MyConversionSymbol = "L2G";
string OtherConversionSymbol = "G2L";
string myMoneySymbol = "L$";
string otherMoneySymbol = "G$";

integer minAmount = 500;


list busywith = [];
list channel = [];
list listener = [];
list timers = [];
list amountOf = [];
list nameOfUser = [];
integer channelMain = 0;
integer listnerMain = 0;
list hasRanTransaction = [];
integer SERVICE_NAME2KEY        =   19790;
integer SERVICE_NAME2KEY_RET    =   19791;

integer moneyAmountLookup = 0;
string transactionIDLookup = "";

Clean(integer spot)
{
    llListenRemove(llList2Integer(listener, spot));
    busywith = llDeleteSubList(busywith, spot, spot);
    channel = llDeleteSubList(channel, spot, spot);
    listener = llDeleteSubList(listener, spot, spot);
    timers = llDeleteSubList(timers, spot, spot);
    amountOf = llDeleteSubList(amountOf, spot, spot);
    nameOfUser = llDeleteSubList(amountOf, spot, spot);
}

integer randInt(integer n)
{
     return (integer)llFrand(n + 1);
}

integer randIntBetween(integer min, integer max)
{
    return min + randInt(max - min);
}

AskSay(integer temp, key id)
{
    llInstantMessage(id, "On channel " + (string)temp + " please say your avatar name in " + OtherGridName);
    llInstantMessage(id, "For example \"/" + (string)temp + " Your Name\" without quotes");
}

// Take a timestamp and return it in PST string format
// Example of the output format is "Sun 7/15/2007 1:02 pm"
string timestamp2PST(string timestamp) {
    // Set up constants required for the function (could be made global)
    list weekday_names = [ "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" ];
    list days_in_month = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
    
    // Parse the timestamp
    list parts = llParseString2List(timestamp, ["-", "T", ":", ".", "Z"], []);
    
    // If it's not a timestamp, then just return the original string
    if (llGetListLength(parts) != 7) {
        return timestamp;
    } else {
        // Set up the local variables we need
        integer year = llList2Integer(parts, 0);
        integer month = llList2Integer(parts, 1);
        integer day = llList2Integer(parts, 2);
        integer hour = llList2Integer(parts, 3);
        integer minute = llList2Integer(parts, 4);
        integer second = llList2Integer(parts, 5);
        
        // Account for time difference from simulator to UTC->PST
        integer difference = (integer)((llGetWallclock() - llGetGMTclock()) / 3600);
        if (difference > 0) {
            difference -=24;
        }        
        hour += difference;

        // Deal with day/month/year boundaries crossed by the adjustment for the time difference
        if (hour < 0) {
            day--;
            hour += 24;
            if (day <= 0) {
                month--;
                if (month <= 0) {
                    year--;
                    month += 12;
                }

                day = llList2Integer(days_in_month, month - 1);
                
                // Do a leap year adjustment if required
                if (month == 2 && year % 4 == 0 && (year % 100 != 0 || year % 400 == 0)) {
                    day++;
                }
            }
        }
        
        // Determine the day of the week
        // This highly non-intuitive code is based on an algorithm 
        // from http://mathforum.org/library/drmath/view/55837.html
        if (month < 3) {
            month += 12; 
            --year;
        }
        integer weekday_number = (day + 2 * month + (3 * (month + 1)) / 5
            + year + year / 4 - year / 100 + year / 400 + 2) % 7;
        
        if (month > 12) {
            month -= 12;
            ++year;
        }
    
        // Build the PST date string (fixed format)
        string date = llGetSubString(llList2String(weekday_names, weekday_number), 0, 2);
        date += " " + (string)month + "/" + (string)day + "/" + (string)year;
        
        // Convert the hour to am/pm
        string suffix = " pm";
        if (hour < 12) {
            suffix = " am";
            if (hour == 0) {
                hour = 12;
            }
        } else if (hour > 12) {
            hour %= 12;
        }
        
        date += " " + (string)hour + ":";
        if (minute < 10) {
            date += "0" + (string)minute;
        } else {
            date += (string)minute;
        }
        date += suffix;
        return date;
    }
}

default
{    
    state_entry()
    {
        llRequestPermissions(llGetOwner(), PERMISSION_DEBIT );
    }
    
    touch_start(integer number)
    { 
        if (llDetectedKey(0) == llGetOwner())
        {
            state maintenance;
        }
        integer spot = llListFindList(busywith, [(string)llDetectedKey(0)]);
        if (spot == -1)
            llInstantMessage(llDetectedKey(0), "To use the atm, just pay the amount you would like to move to Secondlife");
        else
            llInstantMessage(llDetectedKey(0), "Please follow the instruction laid out in chat.");
    }
    
    money(key id, integer amount)
    {
        if (amount >= 500)
        {
            busywith += [(string)id];
            integer temp = randIntBetween(101, 999);
            channel += [temp];
            listener += [llListen(temp, "", id, "")];
            timers += [llGetUnixTime() + 120];
            amountOf += [amount];
            nameOfUser += [""];
            AskSay(temp, id);
            llSetTimerEvent(125);
        }           
        else
        {
            llInstantMessage(id, "There is a min amount of " + (string)minAmount);
            llGiveMoney(id, amount);
        }
    }
    
    listen( integer channel, string name, key id, string msg )
    {
        integer spot = llListFindList(busywith, [(string)id]);
        if (msg == "CORRECT")
        {
            integer amount = llList2Integer(amountOf, spot);
            string message = "";
            key transactionID = llGetNotecardLine("temp", 0); 
            string timest = timestamp2PST(llGetTimestamp());
            message += "Transfer from " + ThisGridName + " to " + OtherGridName + " " + timest + " \n";
            message += ThisGridName + " User: " + name + " \n";
            message += OtherGridName + " user: " + llList2String(nameOfUser, spot) + " \n";
            message += myMoneySymbol + ": " + (string)amount + " \n";
            message += otherMoneySymbol + ": " + (string)llFloor((float)amount / 2.0) + " \n";
            message += myMoneySymbol + " to " + otherMoneySymbol + " \n";
            message += "Payout Code|" + MyConversionSymbol + "|" +  (string)transactionID + "|" + llList2String(nameOfUser, spot) + "|" + (string)amount + "|" + (string)llFloor((float)amount * 2.0);
            llInstantMessage(id, "Processing Request -- Details are as followed: \n" + message);
            // osMakeNotecard(name + " " + timest, [message]);
            llEmail(notifyEmail, "ATM Deposit", message );
            llInstantMessage(id, "Thanks you. It can take up to 24 hours for you to recienve you Lindens. Please be patient.");
            Clean(spot);
            llSetTimerEvent(0);
        }
        else if (msg == "NOT CORRECT")
        {
            AskSay(channel, id);
        }
        else if (spot != -1)
        {
            if (llSubStringIndex(msg, "|") != -1) 
            {
                llInstantMessage(id, "Yes, nice try asshole... ");
                AskSay(channel, id);
                return;
            }
            nameOfUser = llListReplaceList(nameOfUser, [msg], spot, spot);
            llDialog( id, "Your " + OtherGridName + " Username is " + msg, ["CORRECT", "NOT CORRECT"], channel );
            llInstantMessage(id, "Please answer the dialog box at the top right of your screen.");
        }
        else
        {
            llInstantMessage(id, "There was a error, please contant support");   
        }
    }
    
    timer()
    {
        integer i;
        integer size = llGetListLength(timers);
        if (size >= 1)
        {
            for (i = 0; i < size; i++)
            {
                if (llList2Integer(timers, i) < llGetUnixTime())
                {
                    llInstantMessage((key)llList2String(busywith, i), "ATM timed out. Please try again later.");
                    llGiveMoney((key)llList2String(busywith, i), llList2Integer(amountOf, i));
                    Clean(i);
                    return;
                }
            }
        }
        else if (size == 0)
            llSetTimerEvent(0);
    }
    
    dataserver(key query_id, string data) {
    }
    
    on_rez(integer a)
    {
        llResetScript();
    }
}

state maintenance
{
    state_entry()
    {
        llSetText("maintenance", <0,0,0>, 1.0);
        channelMain = randIntBetween(100, 999);
        listnerMain = llListen(channelMain, "", llGetOwner(), "");
        llSay(0, "I am down for maintenance.");
        llDialog(llGetOwner(), "Please select a option.", ["Test Email", "Send Payouts", "Finished"], channelMain);
    }
    
    state_exit()
    {
        llSetText("", <0,0,0>, 1.0);
        llListenRemove(listnerMain);
        hasRanTransaction = [];
    }
    
    touch_start(integer b)
    {
        key a = llDetectedKey(0);
        if (a != llGetOwner())
        {
            llInstantMessage(llDetectedKey(0), "Sorry, but this ATM is down for maintenance");
        }
        else
        {
            llDialog(a, "Please select a option.", ["Test Email", "Send Payouts", "Finished"], channelMain);
        }
    }
    
    link_message(integer s, integer n, string m, key avatarKey)
    {
        if(n == SERVICE_NAME2KEY_RET) {
            if(avatarKey == NULL_KEY)
            {
                llOwnerSay("User " + m + " can not be found. Please manually pay out try to figure out who she/he is and pay them.");
            }
            else
            {
                llGiveMoney(avatarKey, moneyAmountLookup);
                hasRanTransaction += [transactionIDLookup];
                llOwnerSay("Complete.. Ready for next.");
            }
        }
    }
    
    listen( integer channel, string name, key id, string msg )
    {
        if (id != llGetOwner()) return;
        if (msg == "Send Payouts")
        {
            llOwnerSay("Please say the payout codes on channel " + (string)channelMain);
        }
        else if (msg == "Test Email")
        {
            llOwnerSay("Testing Email... Please stand by");
            llEmail(notifyEmail, "ATM TEST", "This is only a test.");
            llOwnerSay("Test email sent.");
        }
        else if (msg == "Finished")
        {
            state default;
        }
        else
        {
            list results = llParseString2List(msg,["|"],[]);
            if (llList2String(results, 0) == "Payout Code")
            {
                if (llList2String(results, 1) == MyConversionSymbol)
                {
                    llOwnerSay("You need to run this code in " + OtherGridName);
                }
                else if (llList2String(results, 1) == OtherConversionSymbol)
                {
                    if (llGetListLength(results) <= 4)
                    {
                        llOwnerSay("Not a full and complete code. Are you sure you copied it over correctly");
                    }
                    else if (llListFindList(hasRanTransaction, [llList2String(results, 2)]) != -1)
                    {
                        llOwnerSay("You have already ran this transactions.");
                    }
                    else
                    {
                        moneyAmountLookup = (integer)llList2String(results, 5);
                        transactionIDLookup = llList2String(results, 2);
                        llMessageLinked(LINK_THIS, SERVICE_NAME2KEY, llList2String(results, 3), "");
                    }
                }
            }
        }
    }
}