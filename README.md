# Avaliação Cedro - BackEnd

## Observações gerais
O código a seguir está dividido em um projeto de teste (test-api) e um projeto de webapi para a avaliação (api). O escopo dos testes foram basicamente na estruturação correta dos códigos e avaliação unitária dos resultados. Os testes de integração necessários para a avaliação de performance, gestão de erros e funcionamento junto à infraestrutura não foram implementados por não estarem no escopo inicial da avaliação.

## Tecnologias e padrões utilizados
O código é implementado em C# (netcoreapp2.1) criando um WebApi REST seguindo padrões da indústria para os verbos e respostas HTTP para simplificação da integração com outros sistemas. 

###### Modelo HTTP
* Os endpoints seguem o padrão `/api/[entidades]` onde `[entidades]` é o plural da entidade a ser registrada.
* Para CRIAÇÃO de uma entidade, é utilizado o verbo http `POST` junto ao endpoint utilizando um body `application/json` estruturado como a entidade. O id é ignorado e gerado pelo próprio serviço. Os retornos possíveis são `400 (BAD REQUEST)` em caso de erros de validação da entidade enviada ou `201 (CREATED)` em caso de sucesso. A nova entidade é retornada no body da resposta de sucesso.
* Para ALTERAÇÃO de uma entidade, é utilizado o verbo http `PUT` junto ao endpoint utilizando um body `application/json` estruturado como a entidade. O id é requerido. Os retornos possíveis são `400 (BAD REQUEST)` em caso de erros de validação da entidade enviada, `404 (NOTFOUND)` em caso de id não encontrado ou `200 (OK)` em caso de sucesso. A entidade alterada é retornada no body da resposta de sucesso.
* Para EXCLUSÃO de uma entidade, é utilizado o verbo http `DELETE` junto ao endpoint anexado ao id `/api/[entidades]/[id]`. Os retornos possíveis são `404 (NOTFOUND)` em caso de id não encontrado ou `204 (NOCONTENT)` em caso de sucesso. Nenhum valor é retornado em caso de sucesso.
* Para a busca de uma única entidade, é utilizado o verbo http `GET` junto ao endpoint anexado ao id `/api/[entidades]/[id]`. Os retornos possíveis são `404 (NOTFOUND)` em caso de id não encontrado ou `200 (OK)` em caso de sucesso. A entidade encontrada é retornada no body da resposta de sucesso.
* Para a busca de uma lista de entidades, é utilizado o verbo http `GET` junto ao endpoint. Somente o retorno `200 (OK)` é possível, onde serão retornadas no corpo da resposta as entidades encontradas ou uma lista vazia em caso de não existir nenhuma.
* Para o endpoint `/api/restaurantes` é possível enviar o parâmetro `filtro` para a seleção das entidades a serem retornadas. O conteúdo do filtro é utilizado para a pesquisa na propriedade `nome` da entidade. Os trechos enviados são pesquisados de forma independente o que torna possível a busca de resultados em qualquer ponto do nome desde que todos os trechos sejam encontrados em algum ponto do mesmo. A pesquisa é insensível ao caso.

###### Exclusão Lógica
Não há exclusão física dos registros. O padrão utilizado para a remoção de entidades é através da flag "Excluido" e filtragem padrão para todas as entidades. No caso de exclusões em cascata, a decisão para a exclusão das entidades envolvidas é tomada pelo serviço responsável pela entidade principal.

###### Arquitetura do projeto
O projeto é dividido em Controllers e Serviços onde cada serviço atende a uma única entidade e cada controller se utiliza de um único serviço para a comunicação. Tanto os serviços quanto os controllers possuem classes base abstratas que devem ser seguidas e que permitem ter seus comportamentos modificados pelas implementações finais em caso de necessidade.

###### Entidades e Modelos auxiliares
Para atender à possíveis discrepâncias entre a entidade persistida e os objetos de requests e responses da API, implementei modelos intermediários para a comunicação que são mapeados automaticamente com as entidades. Para este mapeamento foi utilizado o `AutoMapper` para a simplicação do processo.
