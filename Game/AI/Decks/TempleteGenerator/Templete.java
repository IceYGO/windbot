import Cards.CardTypes.DefaultCard;
import Cards.CardTypes.Monsters.*;
import Cards.CardTypes.TrapOrSpell;
import Cards.YGOCard;
import Cards.YGODeck;

import java.util.List;
import java.util.function.Function;

public class Templete {
    YGODeck deck;

    public Templete(String deckName,
                    String aiName,
                    List<LinkMonster> linkMonsters,
                    List<NormalMonster> normalMonsters,
                    List<EffectMonster> effectMonsters,
                    List<PureSpecialSummonMonster> pureSpecialSummonMonsters,
                    List<SpecialSummonableEffectMonster> specialSummonableEffectMonsters,
                    List<TrapOrSpell> trapsAndSpells,
                    List<DefaultCard> useless) {
        this.deck = new YGODeck(deckName,aiName,linkMonsters, useless,normalMonsters,effectMonsters,pureSpecialSummonMonsters,specialSummonableEffectMonsters,trapsAndSpells);
    }

    public Templete(YGODeck deck) {
        this.deck = deck;
        System.out.println("Number of normals:  " + this.deck.getNormalMonsters());
        System.out.println("Number of effectMonsters:  " + this.deck.getEffectMonsters());
        System.out.println("Number of pureSpecialSummonMonsters:  " + this.deck.getPureSpecialSummonMonsters());
        System.out.println("Number of specialSummonableEffectMonsters:  " + this.deck.getSpecialSummonableEffectMonsters());
        System.out.println("Number of trapsAndSpells:  " + this.deck.getTrapsAndSpells());
        System.out.println("Number of useless:  " + this.deck.getDefaultCards());
    }

    public String generateTemplate(){
        String templete =
                "using System.Collections.Generic;\n" +
                "using WindBot;\n" +
                "using WindBot.Game;\n" +
                "using WindBot.Game.AI;\n" +
                "using YGOSharp.OCGWrapper.Enums;\n" +
                "namespace WindBot.Game.AI.Decks\n" +
                "{\n" +
                "   \n" +
                "   [Deck(\"" + this.deck.getDeckName() + "\", \"" + this.deck.getAiName() +"\")]\n" +
                "    public class " + this.deck.getAiName() + "Executor: DefaultExecutor\n" +
                "    {\n" +
                "        public class CardId\n" +
                "        {\n" +
                            generateInit() +
                "         }\n" +
                "        public " + this.deck.getDeckName() + "Executor(GameAI ai, Duel duel)\n" +
                "            : base(ai, duel)\n" +
                "        {\n" +
                            generateExecutors() +
                "         }\n" +
                            generateMethods() +
                "    }\n" +
                "}";


        return templete;

    }

    private String generateExecutors(){
        StringBuilder cardInit = new StringBuilder();
        cardInit.append("            // Add Executors to all normal monsters\n");
        cardInit.append(generateExecutors(this.deck.getNormalMonsters()));
        cardInit.append("            // Add Executors to all effect monsters\n");
        cardInit.append(generateExecutors(this.deck.getEffectMonsters()));
        cardInit.append("            //  Add Executors to all special summonable effect monsters\n");
        cardInit.append(generateExecutors(this.deck.getSpecialSummonableEffectMonsters()));
        cardInit.append("            //  Add Executors to all pure special summonable effect monsters\n");
        cardInit.append(generateExecutors(this.deck.getPureSpecialSummonMonsters()));
        cardInit.append("            //  Add Executors to all Link monsters\n");
        cardInit.append(generateExecutors(this.deck.getLinkMonsters()));
        cardInit.append("            //  Add Executors to all spell and trap cards\n");
        cardInit.append(generateExecutors(this.deck.getTrapsAndSpells()));
        cardInit.append("\n");
        return cardInit.toString();
    }
    private String generateInit(){
        StringBuilder cardInit = new StringBuilder();
        cardInit.append("            // Initialize all normal monsters\n");
        cardInit.append(generateInit(this.deck.getNormalMonsters()));
        cardInit.append("            // Initialize all effect monsters\n");
        cardInit.append(generateInit(this.deck.getEffectMonsters()));
        cardInit.append("            // Initialize all special summonable effect monsters\n");
        cardInit.append(generateInit(this.deck.getSpecialSummonableEffectMonsters()));
        cardInit.append("            // Initialize all pure special summonable effect monsters\n");
        cardInit.append(generateInit(this.deck.getPureSpecialSummonMonsters()));
        cardInit.append("            // Initialize all link monsters\n");
        cardInit.append(generateInit(this.deck.getLinkMonsters()));
        cardInit.append("            // Initialize all spell and trap cards\n");
        cardInit.append(generateInit(this.deck.getTrapsAndSpells()));
        cardInit.append("            // Initialize all useless cards\n");
        cardInit.append(generateInit(deck.getDefaultCards()));
        cardInit.append("\n");
        return cardInit.toString();
    }

    private String generateMethods(){
        StringBuilder cardInit = new StringBuilder();
        cardInit.append("\n            // All Normal Monster Methods\n");
        cardInit.append(generateMethods(this.deck.getNormalMonsters()));
        cardInit.append("\n            // All Effect Monster Methods\n");
        cardInit.append(generateMethods(this.deck.getEffectMonsters()));
        cardInit.append("\n            // All Special Summonable Effect Monster Methods\n");
        cardInit.append(generateMethods(this.deck.getSpecialSummonableEffectMonsters()));
        cardInit.append("\n            // All Pure Special Summonable Effect Monster Methods\n");
        cardInit.append(generateMethods(this.deck.getPureSpecialSummonMonsters()));
        cardInit.append("\n            // All Link Monster Methods\n");
        cardInit.append(generateMethods(this.deck.getLinkMonsters()));
        cardInit.append("\n            // All Spell and Trap Card Methods\n");
        cardInit.append(generateMethods(this.deck.getTrapsAndSpells()));
        cardInit.append("\n");
        return cardInit.toString();
    }
    private String generateInit(List<? extends YGOCard> cards){
        Function<YGOCard, String> lambda = YGOCard::generateInit;
        return generalGenerator(lambda,cards);
    }

    private String generateExecutors(List<? extends YGOCard> cards){
        Function<YGOCard, String> lambda = YGOCard::generateAddExecutorMethods;
        return generalGenerator(lambda,cards);
    }

    private String generateMethods(List<? extends YGOCard> cards){
        Function<YGOCard, String> lambda = YGOCard::generateCardMethods;
        return generalGenerator(lambda,cards);
    }

    private String generalGenerator(Function<YGOCard, String> lambda, List<? extends YGOCard> cards){
        StringBuilder cardInit = new StringBuilder();
        //System.out.println("Number of entities is " + cards.size());
        for (YGOCard card : cards){
          //  cardInit.append("\n            // Create all methods needed for ").append(card.getName()).append(" to work\n");
            cardInit.append(lambda.apply(card));
        }
        return cardInit.toString();
    }

}
