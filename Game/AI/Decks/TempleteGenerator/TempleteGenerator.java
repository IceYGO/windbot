import Cards.CardIdentifyer.CDBParser;
import Cards.CardTypes.DefaultCard;
import Cards.CardTypes.Monsters.*;
import Cards.CardTypes.TrapOrSpell;
import Cards.YGODeck;

import java.io.*;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.List;
import java.util.Objects;


public class TempleteGenerator {

    static String filePath = "Game/AI/Decks/TempleteGenerator/UserFiles/results/";// CHANGE THIS PATH TO WHERE YOU WANT THE FILE TO OUTPUT TO
    List<DefaultCard> defaultCards = new ArrayList<>();
    List<LinkMonster> linkMonsters = new ArrayList<>();
    List<NormalMonster> normalMonsters = new ArrayList<>();
    List<EffectMonster> effectMonsters = new ArrayList<>();
    List<PureSpecialSummonMonster> pureSpecialSummonMonsters = new ArrayList<>();
    List<SpecialSummonableEffectMonster> specialSummonableEffectMonsters = new ArrayList<>();
    List<TrapOrSpell> trapsAndSpells = new ArrayList<>();
    String deckName;
    String aiName;
    CDBParser parser;

    public TempleteGenerator(String deckName, String aiName) {
        try {
            Class.forName("org.sqlite.JDBC");
            String cdbPath = "src/main/java/UserFiles/cards.cdb";
            addDefaultCards();
            this.deckName = deckName;
            this.aiName = aiName;
            String path = filePath + deckName + "Executor.cs";
            parser = new CDBParser(cdbPath);
            generate(path);
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        }


    }


    public TempleteGenerator() {
        try {
            Class.forName("org.sqlite.JDBC");
            String cdbPath = "Game/AI/Decks/TempleteGenerator/UserFiles/cards.cdb";
            parser = new CDBParser(cdbPath);
            generateAiTempletes();
        } catch (ClassNotFoundException e) {
            e.printStackTrace();
        }


    }




    private void addNormalMonsters(){
        this.normalMonsters.add(new NormalMonster("ElementalHERONeos",89943723));
    }

    //  Effect monsters that can't be special summoned through their own effect (e.g. most effect monsters)
    private void addEffectMonsters(){
        /*
        this.effectMonsters.add(new EffectMonster("ElementalHEROHonestNeos", 14124483));
        this.effectMonsters.add(new EffectMonster("ElementalHERONeosAlius", 69884162));
        this.effectMonsters.add(new EffectMonster("NeoSpacePathFinder", 19594506));
        this.effectMonsters.add(new EffectMonster("ElementalHEROStratos", 40044918));
        this.effectMonsters.add(new EffectMonster("ElementalHEROPrisma", 89312388));
        this.effectMonsters.add(new EffectMonster("ElementalHEROBlazeman", 63060238));
        this.effectMonsters.add(new EffectMonster("NeoSpaceConnector", 85840608));
        this.effectMonsters.add(new EffectMonster("NeoSpacienGrandMole", 80344569));
        this.effectMonsters.add(new EffectMonster("NeoSpacianAirHummingbird", 54959865));
        this.effectMonsters.add(new EffectMonster("NeoSpacianAquaDolphin", 17955766));
        this.effectMonsters.add(new EffectMonster("NeoSpacianFlareScarab", 89621922));
         */
    }

    //  Monsters that cannot be normal summoned (e.g. Extra Deck Monsters)
    private void addPureSpecialSummonMonsters(){
        /*
        this.pureSpecialSummonMonsters.add(new PureSpecialSummonMonster("ElementalHEROStormNeos", 49352945));
        this.pureSpecialSummonMonsters.add(new PureSpecialSummonMonster("ElementalHeroMagmaNeos", 78512663));
        this.pureSpecialSummonMonsters.add(new PureSpecialSummonMonster("ElementalHeroAirNeos", 11502550));
        this.pureSpecialSummonMonsters.add(new PureSpecialSummonMonster("ElementalHeroGrandNeos", 48996569));
        this.pureSpecialSummonMonsters.add(new PureSpecialSummonMonster("ElementalHeroAquaNeos", 55171412));
        this.pureSpecialSummonMonsters.add(new PureSpecialSummonMonster("ElementalHeroBraveNeos", 64655485));
        this.pureSpecialSummonMonsters.add(new PureSpecialSummonMonster("ElementalHeroFlareNeos", 81566151));
        this.pureSpecialSummonMonsters.add(new PureSpecialSummonMonster("ElementalHeroSunrise", 22908820));
        this.pureSpecialSummonMonsters.add(new PureSpecialSummonMonster("ElementalHeroNeosKnight", 72926163));

         */
    }

    //  Monsters that can be normal or special summoned
    private void addSpecialSummonableEffectMonsters(){

    }

    private void addTrapsAndSpells(){
        /*
        this.trapsAndSpells.add(new TrapOrSpell("MonsterReborn", 83764718));
        this.trapsAndSpells.add(new TrapOrSpell("NeosFusion", 14088859));
        this.trapsAndSpells.add(new TrapOrSpell("Polymerization", 24094653));
        this.trapsAndSpells.add(new TrapOrSpell("MiracleContact", 35255456));
        this.trapsAndSpells.add(new TrapOrSpell("InstantNeoSpace", 11913700));
        this.trapsAndSpells.add(new TrapOrSpell("NeoSpace", 42015635));
        this.trapsAndSpells.add(new TrapOrSpell("DrowningMirrorForce", 47475363));
        this.trapsAndSpells.add(new TrapOrSpell("NEXT", 74414885));
        this.trapsAndSpells.add(new TrapOrSpell("CallOfTheHaunted", 97077563));

         */
    }



    //  For when you want to check monsters you don't own, or just have them defined for some reason
    private void addDefaultCards(){
        /*
        try {
            this.defaultCards.add(parser.getCard(14558127));
            this.defaultCards.add(parser.getCard(62015408));
            this.defaultCards.add(parser.getCard(38814750));
            this.defaultCards.add(parser.getCard(94145021));
            this.defaultCards.add(parser.getCard(34267821));
            this.defaultCards.add(parser.getCard(59438930));
            this.defaultCards.add(parser.getCard(27204311));
            this.defaultCards.add(parser.getCard(97268402));
            this.defaultCards.add(parser.getCard(24508238));
            this.defaultCards.add(parser.getCard(37742478));
            this.defaultCards.add(parser.getCard(877036));
            this.defaultCards.add(parser.getCard(44330098));
        } catch (SQLException e) {
            e.printStackTrace();
        }
        */
    }

    public void generate(String filePath){

        try {
            File file = new File(filePath);
            file.createNewFile();
            FileWriter output = new FileWriter(file);
            String generatedtemplete =
                    new Templete(
                            this.deckName,
                            this.aiName,
                            this.linkMonsters,
                            this.normalMonsters,
                            this.effectMonsters,
                            this.pureSpecialSummonMonsters,
                            this.specialSummonableEffectMonsters,
                            this.trapsAndSpells,
                            this.defaultCards
                                        ).generateTemplate();
            output.write(generatedtemplete);
            output.close();
        } catch (IOException e) {
            System.out.println("Failed");
            e.printStackTrace();
        }

    }
    public void generate(YGODeck deck){

        try {
            String path = filePath + deck.getDeckName().replaceAll(" ", "") + "Executor.cs";
            File file = new File(path);
            file.createNewFile();
            FileWriter output = new FileWriter(file);
            String generatedtemplete =
                    new Templete(deck).generateTemplate();
            output.write(generatedtemplete);
            output.close();
        } catch (IOException e) {
            System.out.println("Failed");
            e.printStackTrace();
        }

    }

    private List<Integer> decklist(File file) throws IOException {

        BufferedReader reader = new BufferedReader(new FileReader(file));
        List<Integer> ids = new ArrayList<>();
        int lineCounter = 0;
        String line = reader.readLine();
        line = reader.readLine();
        line = reader.readLine();
        while (line != null && !line.contains("!side")){

            if(line.contains("!default")){
                ids.add(0);
              line =  reader.readLine();
            }
            else if(line.contains("#extra")) {
                line = reader.readLine();
            }
            else{

                    line = line.replaceAll(" ", "");
                    line = line.replaceAll("\n", "");
                    try{
                        int id = Integer.parseInt(line);
                        if(!ids.contains(id)){
                            ids.add(id);
                        }
                    }
                    catch (Exception e){
                        e.printStackTrace();
                    }

                    line = reader.readLine();
            }

        }
        reader.close();
        return ids;
    }


    private YGODeck getYGODeck(File file){
        YGODeck deck = new YGODeck();
        try {
            List<Integer> ids = decklist(file);
            if(ids.contains(0)){
                for (int x = 0; x < ids.get(0); x++){
                    System.out.println(x);
                    parser.addCard(ids.get(x));
                }
                for (int y = ids.get(0) + 1; y < ids.size(); y++){
                    System.out.println(y);
                    parser.addDefaultCard(ids.get(y));
                }
                System.out.println("Got here");
            }
            else{
                for(int id: ids){
                    parser.addCard(id);
                }
            }


            deck = parser.getYGODeck();
            System.out.println( "Printing traps :" + deck.getTrapsAndSpells());
            String name = file.getName().replaceAll(".ydk", "");
            name = name.replaceAll( "[^a-zA-Z0-9]","");
            name = name.replaceAll("!","");
            deck.setAiName(name);
            name += "Executor.cs";
            deck.setDeckName(name);

        } catch (IOException | SQLException e) {
            e.printStackTrace();
        }
        return deck;
    }

    public void generateAiTempletes(){
        File folder = new File("Game/AI/Decks/TempleteGenerator/UserFiles/decks/");
        for(File f: Objects.requireNonNull(folder.listFiles())){
            if(f.getName().contains(".ydk")){

                generateAiTemplete(f);
            }
        }
    }

    public void generateAiTemplete(File f){

        YGODeck deck = getYGODeck(f);
        String name = f.getName().replaceAll(".ydk", "");
        deck.setDeckName(name);

        generate(deck);
    }


    public CDBParser getParser() {
        return parser;
    }

    /*
    Extra cards for neos
    !default
14558127
62015408
38814750
94145021
34267821
59438930
27204311
97268402
24508238
37742478
98777036
44330098
     */
}
