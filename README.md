# F360Task - Sistema de Mensageria com MongoDB, RabbitMQ e ASP.NET Core

Este projeto orquestra múltiplos serviços que interagem via RabbitMQ para envio de e-mails e geração de relatórios, utilizando MongoDB como banco de dados e aplicações ASP.NET Core como consumidores e APIs.

## 🧱 Estrutura dos Serviços

### Banco de Dados
- **MongoDB**  
  - Porta: `27017`
  - Usuário/padrão: `${MONGO_USER:-admin}`
  - Senha/padrão: `${MONGO_PASS:-admin123}`

### Mensageria
- **RabbitMQ**  
  - Porta AMQP: `5672`  
  - UI de gerenciamento: `http://localhost:15672`  
  - Usuário/padrão: `${RABBITMQ_USER:-guest}`  
  - Senha/padrão: `${RABBITMQ_PASS:-guest}`

### APIs
- **f360taskapiemail**  
  - Porta: `5000` (HTTP), `5001` (alternativa)
  - Finalidade: Recebe requisições de envio de e-mails.

- **f360taskapireport**  
  - Porta: `5010` (HTTP), `5011` (alternativa)
  - Finalidade: Recebe requisições de geração de relatórios.

### Consumers
- **f360taskconsumeremail**  
  - Função: Consome mensagens da fila `EnviarEmail` e processa envios de e-mails.

- **f360taskconsumerreport**  
  - Função: Consome mensagens da fila `GerarReport` e processa geração de relatórios.

## 📨 Recebimento de Tarefas (Jobs)

As aplicações `f360taskapiemail` e `f360taskapireport` expõem endpoints de API REST que permitem o **registro de novas tarefas para processamento assíncrono**.

### Justificativa

A arquitetura adotada separa a responsabilidade de **recebimento de requisições** da lógica de **processamento**, utilizando RabbitMQ como intermediário. Isso traz os seguintes benefícios:

- ✅ **Alta escalabilidade:** múltiplos consumidores podem ser adicionados conforme a demanda.
- ✅ **Maior resiliência:** falhas no processamento não impactam diretamente a recepção da tarefa.
- ✅ **Desacoplamento:** facilita a manutenção, testes e substituição de componentes.
- ✅ **Responsividade:** a API responde rapidamente sem aguardar o término do processamento.

> ⚠️ Observação: O foco desta aplicação é estrutural. O processamento real (envio de e-mail ou geração de relatório) **não está implementado**; o objetivo é demonstrar como tarefas seriam enfileiradas e processadas em segundo plano.

## 🔧 Variáveis de Ambiente

As seguintes variáveis podem ser definidas via `.env` para configurar os serviços:

```env
# MongoDB
MONGO_USER=admin
MONGO_PASS=admin123
MONGO_URI_EMAIL=mongodb://admin:admin123@mongodb:27017/email_db
MONGO_URI_REPORT=mongodb://admin:admin123@mongodb:27017/report_db

# RabbitMQ
RABBITMQ_USER=guest
RABBITMQ_PASS=guest
▶️ Executando o Projeto
Clone o repositório:

bash
Copiar
Editar
git clone https://seu-repositorio.git
cd seu-repositorio
Crie um arquivo .env na raiz e configure as variáveis conforme necessário.

Suba os containers com Docker Compose:

bash
Copiar
Editar
docker-compose up --build
Acesse os serviços:

RabbitMQ UI: http://localhost:15672

API Email: http://localhost:5000

API Report: http://localhost:5010

🧪 Healthchecks
Todos os serviços críticos incluem healthcheck para garantir inicialização segura e dependência saudável entre containers.

📦 Volumes e Persistência
Volume mongodb_data mapeado em /data/db garante persistência dos dados do MongoDB entre reinicializações.

📡 Rede
Todos os serviços estão interligados na rede backend, garantindo comunicação interna isolada via container_name.

📄 Licença
MIT

✉️ Contato
Para dúvidas ou sugestões, entre em contato: seu.email@exemplo.com

yaml
Copiar
Editar

---

Se precisar, posso ajudar a adaptar ou incluir mais alguma coisa! Quer que eu te ajude com algo mais?


