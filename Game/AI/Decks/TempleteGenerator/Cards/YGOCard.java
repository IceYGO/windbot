package Cards;

public abstract class YGOCard {
    protected int id;
    protected String name;
    protected String spellSetMethodName;
    protected String activateMethodName;
    protected String monsterSetMethodName;
    protected String spSummonMethodName;
    protected String normalSummonMethodName;
    protected String reposMethodName;


    public YGOCard(String name, int id) {
        this.id = id;
        this.name = name;
        this.spellSetMethodName = this.name + "SpellSet";
        this.activateMethodName = this.name + "Activate";
        this.monsterSetMethodName = this.name + "MonsterSet";
        this.spSummonMethodName = this.name + "SpSummon";
        this.normalSummonMethodName = this.name + "NormalSummon";
        this.reposMethodName = this.name + "Repos";
    }

    public int getId() {
        return id;
    }

    public String getName() {
        return name;
    }

    public abstract String generateAddExecutorMethods();
    public abstract String generateCardMethods();

    protected String generateSpellSetExecutor(){
        return generateExecutor("SpellSet", this.spellSetMethodName);
    }
    protected String generateSummonExecutor(){
        return generateExecutor("Summon", this.normalSummonMethodName);
    }

    protected String generateMonsterSetExecutor(){
        return generateExecutor("MonsterSet", this.monsterSetMethodName);
    }

    protected String generateSpSummonExecutor(){
        return generateExecutor("SpSummon", this.spSummonMethodName);
    }

    protected String generateActivateExecutor(){
        return generateExecutor("Activate", this.activateMethodName);
    }

    protected String generateReposExecutor(){
        return generateExecutor("Repos", this.reposMethodName);
    }

    protected String generateSpellSetMethod(){
       return generateMethod(spellSetMethodName);
    }
    protected String generateReposMethod(){
        return generateMethod(reposMethodName);
    }
    protected String generateSummonMethod(){
        return generateMethod(normalSummonMethodName);
    }
    protected String generateMonsterSetMethod(){
        return generateMethod(monsterSetMethodName);
    }
    protected String generateSpSummonMethod(){
        return generateMethod(spSummonMethodName);
    }
    protected String generateActivateMethod(){
        return generateMethod(activateMethodName);
    }

    private String generateMethod(String methodName){
        String method ="\n"+
                "        private bool " + methodName + "()\n" +
                "        {\n\n" +
                "            return ";
        if(methodName.equals(reposMethodName)){
            method += "DefaultMonsterRepos;\n";
        }
        else if(methodName.equals(spellSetMethodName)){
            method += "DefaultSpellSet;\n";
        }
        else {
            method += "true;\n";
        }

        method += "        }\n";
        return method;
    }

    /*
 private bool CrystalWingSynchroDragonSummon()
        {
            return !Bot.HasInHand(CardId.AssaultModeActivate)
                && !Bot.HasInHand(CardId.AssaultBeast)
                && !Bot.HasInSpellZone(CardId.AssaultModeActivate);
        }     */

    public String generateExecutor(String executorType, String methodName){
        return "            AddExecutor(ExecutorType." + executorType + ", CardId." + this.name + ", " + methodName +");\n";
    }

    public String generateInit(){
        return "            public const int " + this.name + " = " + this.id + ";\n";
    }

}
