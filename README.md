# polymorphic-request-with-json-schema
PoC sobre possibilitar que um endpoint de uma API possa receber diferentes payloads e consiga interpretá-los corretamente fazendo uso de JSON Schemas com .NET. 

Além disso foi um exercício para construir o código usando somente agente de IA.

## Resumo


### Sobre IA
- O projeto foi criado usando [Developer Container](https://containers.dev/) por preocupações com segurança.
- A experiência foi baseada no relato feito pelo Fábio Akita em [seu blog](https://akitaonrails.com/2026/02/20/do-zero-a-pos-producao-em-1-semana-como-usar-ia-em-projetos-de-verdade-bastidores-do-the-m-akita-chronicles/).
- Usei o agente Claude Code com a versão Pró
- Assim como no relato, foi feito um fluxo de TDD com XP, onde o agente escreve todo o código e eu informo o contexto (Pair Programming).
- Adicionei skills que foram usadas para agilizar o processo. Não precisei escrever o mesmo prompt várias vezes, acionei a skill passando o contexto e deixava o Claude executar.
- Dentro do meu papel, revisava todo o código que era gerado em cada passo e só passava para o próximo item se tudo estivesse minimamente OK.

### Sobre a PoC
- A PoC valida uma possível solução para um tipo de cenário onde um mesmo endpoint precisaria receber payloads dinâmicos e ser capaz de interpretá-los corretamente.
- O foco foi exclusivamente nesse comportamento. O que a aplicação faria após a validação do payload está fora do escopo.
- A estratégia envolve em usar JSON Schemas para identificar se um payload em JSON é válido ou não.
- O endpoint recebe como parâmetro de rota o nome do payload, 
- O serviço tenta encontrar o Schema especificado e caso exista, faz a validação do payload com o Schema.
- Depois é feito o retorno da validação de Falha|Sucesso
- A PoC segue Arquitetura Hexagonal, com Ports e Adapters
- Os testes unitários tambem foram escritos por IA, buscando o máximo de cobertura

## Conclusões

### Sobre solução

- Funcionou como esperado. Json Schema foi armazenado e carregado corretamente. E o código fez a validação como esperado.
- Os testes cobriram bem o código, com somente um arquivo sem teste unitário:
    - [JsonSchemaPayloadValidator](src/Adapters/Driven/Validation/JsonSchemaPayloadValidator.cs)
    - Isso passou tanto pela revisão da IA quanto da minha revisão


### Sobre o uso da IA

- **Contexto é Rei.** 
    - Tudo foi muito fluído mas por causa do arquivo [CLAUDE.md](CLAUDE.md) onde está todo o contexto do projeto.
- **Skill é a chave para repetição.** 
    - Dado que contexto é crucial, para que a IA entregue o mesmo resultado sempre, ela precisa receber o mesmo contexto sempre. O uso de skills agiliza e padroniza o processo.
- **Cuidados com segurança.** 
    - Deixar o agente com acesso ao seu computador sempre trará riscos, ainda mais com os ataques de Supply-Chain que têm aparecido. Rodar via Developer Container trás uma segurança forte. 
    - Eu não permiti o agente rodar git push, mas tambem no momento que precisou eu fiz isso manualmente. É importante definir até onde é seguro a IA mexer.
    - Caso necessite de Keys ou dados importantes, sempre coloque em variáveis e não as deixe expostas tanto no repositório quanto no Container
- **Atenção com o uso de tokens.** 
    - Para esse projeto gastei uma boa cota do limite de tokens. Não cheguei nos 100% mas passei dos 60%. Isso envolveu não somente a execução do agente, mas de algumas dúvidas que tirei com o Claude em outras sessões. 
- **Agilidade.** 
    - Apesar do que considero um alto consumo de tokens, o projeto foi feito em umas 3 horas. Entre criar repositório, criar estrutura e configuração inicial (CLAUDE.md, pasta src, devcontainer), iniciar e rodar container, construir a aplicação, revisando alguns tópicos e tirando dúvidas durante o desenvolvimento, testar a API rodando localmente e fazer o push ao repositório (isso de forma manual). **Se bem preparado, o uso da IA lhe trará um bom retorno.**


## Aprendizados para o futuro

- Refinar Contextos e Skills para aumentar a eficiência (resultado por consumo de tokens). Isso envolve deixar os documentos mais concisos, coesos e mais focados no escopo do projeto.
- Deixar o Claude somente para o agente. Usar outras IAs para análises, dúvidas e planejamentos 
- Evoluir Container Development. Adicionar ferramentas e extensões que façam sentido ao projeto. Em alguns momentos o VS Code se "perdeu" pois o escopo dele é somente o ambiente do container
- O uso de JSON Schema permite um bom dinamismo, mas é importante prestar atenção nos trade-offs. Uma API de alta performance precisará dessa validação em alta velocidade. Soluções de cache e aumento de performance na validação serão cruciais