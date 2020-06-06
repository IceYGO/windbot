package Cards.CardIdentifyer;

import Cards.CardTypes.DefaultCard;
import Cards.CardTypes.Monsters.*;
import Cards.CardTypes.TrapOrSpell;
import Cards.YGODeck;

import java.sql.*;
import java.util.Arrays;
import java.util.List;

import static java.lang.System.exit;

public class CDBParser extends YGODeck{
    private String cdbPath;
    private Connection connection;

    public CDBParser(String cdbPath){
        this.cdbPath = cdbPath;
        try
        {
            // create a database connection
            connection = DriverManager.getConnection("jdbc:sqlite:" + cdbPath);
            Statement statement = connection.createStatement();
            statement.setQueryTimeout(60);  // set timeout to 30 sec.

            ResultSet rs = statement.executeQuery(
                "SELECT name, datas.id, texts.desc, texts.str1, datas.attribute, datas.category, datas.type, datas.race " +
                     "From texts  " +
                     "inner join datas on datas.id = texts.id");

        }
        catch(SQLException e)
        {
            // if the error message is "out of memory",
            // it probably means no database file is found
            System.err.println(e.getMessage());
        }
    }

    public void addCard(int cardId) throws SQLException {

        String specialChars = "[^a-zA-Z0-9]";
        ResultSet rs = getQuerry(cardId);
        String cardName = rs.getString("name");
        cardName = cardName.replaceAll(specialChars, "");
        int type = rs.getInt("type");
        byte set = rs.getByte("type");
        String str1 = "";
        if(isThere(rs,"str1")){
            str1 = rs.getString("str1");
        }
        System.out.println("Adding " + cardName);
        addCard(type,cardName,cardId,str1);
    }

    private boolean isThere(ResultSet rs, String column){
        try{
            rs.findColumn(column);
            return true;
        } catch (SQLException sqlex){

        }

        return false;
    }

    public void addDefaultCard(int cardId) {
        String specialChars = "[^a-zA-Z0-9]";
        try{
            ResultSet rs = getQuerry(cardId);
            System.out.println(cardId);
            String cardName = rs.getString("name");
            cardName = cardName.replaceAll(specialChars, "");
            int type = rs.getInt("type");
            String str1 = "";

            if(isThere(rs,"str1")){
                str1 = rs.getString("str1");
            }
            this.defaultCards.add(new DefaultCard(cardName,cardId));
            System.out.println("My traps and spells are " + this.trapsAndSpells);
        }
        catch (Exception e){
            System.out.println(e);
            exit(0);
        }

    }

    public DefaultCard getCard(int cardId) throws SQLException {
        String specialChars = "[^a-zA-Z0-9]";
        ResultSet rs = getQuerry(cardId);
        String cardName = rs.getString("name");
        cardName = cardName.replaceAll("[^a-zA-Z0-9]", "");
        int type = rs.getInt("type");
        String str1 = "";
        if(isThere(rs,"str1")){
            str1 = rs.getString("str1");
        }
        return new DefaultCard(cardName,cardId);
    }

    public void close(){
        if(connection != null) {
            try {
                connection.close();
            } catch (SQLException e) {
                e.printStackTrace();
            }
        }
    }

    public ResultSet getQuerry(int cardId) throws SQLException {
        ResultSet rs = null;
        String querry =
                "SELECT texts.name, datas.id, texts.desc, ot, alias, setcode, type, atk, def, level, race, attribute, category " +
                        "From texts  inner join datas on datas.id = texts.id " +
                        "where datas.id = " + cardId;

        Statement s = connection.createStatement();
        rs = s.executeQuery(querry);
        return rs;
    }

    private void addCard(int code, String name, int id, String str1){
        String hexCode = Integer.toHexString(code);
       // System.out.println("Test HEXCODE    " + hexCode);
        String normalHex = "11";
        String effectHex = "21";
        String spellHex = "2";
        String trapHex = "4";
        String linkMonsterHex = "4000000";


        System.out.println(hexCode);
        if((hasSameIndex(hexCode,spellHex,1)) || (hasSameIndex(hexCode,trapHex,1))){
            this.trapsAndSpells.add(new TrapOrSpell(name,id));
        }
        else if((hexCode.length() > 1) && (hasSameIndex(hexCode,normalHex,2))){
            this.normalMonsters.add(new NormalMonster(name,id));
        }
        else if((hexCode.length() > 1 ) && (hasSameIndex(hexCode, effectHex, 2))){
            {
                if(str1.contains("Special Summon")){
                    this.specialSummonableEffectMonsters.add(new SpecialSummonableEffectMonster(name,id));
                }
                else {
                    this.effectMonsters.add(new EffectMonster(name,id));
                }
            }

        }
        else if(isExtradeck(hexCode)){
            this.pureSpecialSummonMonsters.add(new PureSpecialSummonMonster(name,id));
        }
        else if((hasSameStartIndex(hexCode,linkMonsterHex,1)) && (isSameLenght(hexCode, linkMonsterHex))){
            this.linkMonsters.add(new LinkMonster(name,id));
        }
        else{
            System.out.println("Couldn't identify card type:    " + hexCode);
            System.out.println("for card:    " + id);

            this.defaultCards.add(new DefaultCard(name, id));
        }


    }

    private boolean hasSameIndex(String hexcode, String categoryCode, int numberOfLastChars){
       // System.out.println(hexcode.length() - numberOfLastChars);
       // System.out.println("The index string is " + hexcode.substring(hexcode.length() - numberOfLastChars));
       // System.out.println("Tested against " + categoryCode);
        //System.out.println("Is it the same? " + (hexcode.substring(hexcode.length() - numberOfLastChars).equals(categoryCode)));
        return (hexcode.substring(hexcode.length() - numberOfLastChars).equals(categoryCode));
    }
    private boolean isSameLenght(String hexcode, String categoryCode){
        return (hexcode.length() == categoryCode.length());
    }

    private boolean hasSameStartIndex(String hexcode, String categoryCode, int numberOfFirstChars){
        System.out.println(hexcode);
        return (hexcode.substring(0, numberOfFirstChars).equals(categoryCode));
    }
    private boolean isExtradeck(String hexcode){
        List<String> extraDeckHex = Arrays.asList("61","41", "81", "a1", "2000","800000", "1800000", "1002000");
        return (((hexcode.length() > 1) && (hasSameIndex(hexcode, extraDeckHex.get(0),2))) ||
                ((hexcode.length() > 1) && (hasSameIndex(hexcode, extraDeckHex.get(1),2))) ||
                ((hexcode.length() > 1) && (hasSameIndex(hexcode, extraDeckHex.get(2),2))) ||
                ((hexcode.length() > 1) && (hasSameIndex(hexcode, extraDeckHex.get(3),2))) ||
                (
                        (hasSameStartIndex(hexcode,extraDeckHex.get(4), 1))
                                &&
                        (isSameLenght(hexcode, extraDeckHex.get(4)))
                ) ||
                (
                        (hasSameStartIndex(hexcode,extraDeckHex.get(5), 1))
                                &&
                        (isSameLenght(hexcode, extraDeckHex.get(5)))
                ) ||
                (
                        ((hexcode.length() > 1) && (hasSameStartIndex(hexcode,extraDeckHex.get(6),2)))
                                &&
                        (isSameLenght(hexcode, extraDeckHex.get(6)))
                ) ||
                (
                        ((hexcode.length() > 3 ) && (hasSameStartIndex(hexcode,extraDeckHex.get(7),4)))
                                &&
                        (isSameLenght(hexcode, extraDeckHex.get(7)))
                ));
    }

    public YGODeck getYGODeck(){
        System.out.println("My traps and spells are now " + trapsAndSpells);
        YGODeck deck = new YGODeck("", "", this.linkMonsters, this.defaultCards, this.normalMonsters,this.effectMonsters,this.pureSpecialSummonMonsters,this.specialSummonableEffectMonsters,this.trapsAndSpells);
        this.resetDeck();
        return deck;
    }


}
