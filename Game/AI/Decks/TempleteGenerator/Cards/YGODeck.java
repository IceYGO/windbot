package Cards;

import Cards.CardTypes.DefaultCard;
import Cards.CardTypes.Monsters.*;
import Cards.CardTypes.TrapOrSpell;

import java.util.ArrayList;
import java.util.List;

public class YGODeck {
    protected List<DefaultCard> defaultCards = new ArrayList<>();
    protected List<NormalMonster> normalMonsters = new ArrayList<>();
    protected List<EffectMonster> effectMonsters = new ArrayList<>();
    protected List<PureSpecialSummonMonster> pureSpecialSummonMonsters = new ArrayList<>();
    protected List<SpecialSummonableEffectMonster> specialSummonableEffectMonsters = new ArrayList<>();
    protected List<TrapOrSpell> trapsAndSpells = new ArrayList<>();
    protected List<LinkMonster> linkMonsters = new ArrayList<>();
    String deckName = "";
    String aiName = "";

    public YGODeck(String deckName, String aiName, List<LinkMonster> linkMonsters, List<DefaultCard> defaultCards, List<NormalMonster> normalMonsters, List<EffectMonster> effectMonsters, List<PureSpecialSummonMonster> pureSpecialSummonMonsters, List<SpecialSummonableEffectMonster> specialSummonableEffectMonsters, List<TrapOrSpell> trapsAndSpells) {
        this.normalMonsters = normalMonsters;
        this.effectMonsters = effectMonsters;
        this.pureSpecialSummonMonsters = pureSpecialSummonMonsters;
        this.specialSummonableEffectMonsters = specialSummonableEffectMonsters;
        this.trapsAndSpells = trapsAndSpells;
        this.deckName = deckName;
        this.aiName = aiName;
        this.linkMonsters = linkMonsters;
        this.defaultCards = defaultCards;
    }

    public YGODeck() {

    }




    public List<NormalMonster> getNormalMonsters() {
        return normalMonsters;
    }

    public List<EffectMonster> getEffectMonsters() {
        return effectMonsters;
    }

    public List<PureSpecialSummonMonster> getPureSpecialSummonMonsters() {
        return pureSpecialSummonMonsters;
    }

    public List<SpecialSummonableEffectMonster> getSpecialSummonableEffectMonsters() {
        return specialSummonableEffectMonsters;
    }

    public List<TrapOrSpell> getTrapsAndSpells() {
        return trapsAndSpells;
    }

    public void setNormalMonsters(List<NormalMonster> normalMonsters) {
        this.normalMonsters = normalMonsters;
    }

    public void setEffectMonsters(List<EffectMonster> effectMonsters) {
        this.effectMonsters = effectMonsters;
    }

    public void setPureSpecialSummonMonsters(List<PureSpecialSummonMonster> pureSpecialSummonMonsters) {
        this.pureSpecialSummonMonsters = pureSpecialSummonMonsters;
    }

    public void setSpecialSummonableEffectMonsters(List<SpecialSummonableEffectMonster> specialSummonableEffectMonsters) {
        this.specialSummonableEffectMonsters = specialSummonableEffectMonsters;
    }

    public void setTrapsAndSpells(List<TrapOrSpell> trapsAndSpells) {
        this.trapsAndSpells = trapsAndSpells;
    }

    public String getDeckName() {
        return deckName;
    }

    public String getAiName() {
        return aiName;
    }

    public void setDeckName(String deckName) {
        this.deckName = deckName;
    }

    public void setAiName(String aiName) {
        this.aiName = aiName;
    }

    public void resetDeck(){
        this.aiName = "";
        this.deckName = "";
        emptyDeck();
    }

    public void resetDeck(String deckName, String aiName){
        this.deckName = deckName;
        this.aiName = aiName;
        emptyDeck();
    }

    public void emptyDeck(){
        this.linkMonsters = new ArrayList<>();
        this.defaultCards = new ArrayList<>();
        this.normalMonsters = new ArrayList<>();
        this.effectMonsters = new ArrayList<>();
        this.pureSpecialSummonMonsters = new ArrayList<>();
        this.specialSummonableEffectMonsters = new ArrayList<>();
        this.trapsAndSpells = new ArrayList<>();
    }

    public void setLinkMonsters(List<LinkMonster> linkMonsters) {
        this.linkMonsters = linkMonsters;
    }

    public void setDefaultCards(List<DefaultCard> defaultCards) {
        this.defaultCards = defaultCards;
    }

    public List<DefaultCard> getDefaultCards() {
        return defaultCards;
    }

    public List<LinkMonster> getLinkMonsters() {
        return linkMonsters;
    }
}
