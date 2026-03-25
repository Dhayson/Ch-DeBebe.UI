<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router';
import api from '@/services/api';
import { useToast } from 'primevue';

const toast = useToast();
const route = useRoute();
const chaId = route.params.id; // Captura o ID da URL
const detalhes = ref(null);
const loading = ref(true);
import { useConfirm } from "primevue/useconfirm";
import { FileUpload } from 'primevue';

const buscarDetalhes = async () => {
    try {
        // Faz o GET no endpoint que retorna o Chá + Presentes (com aquele Join/Sum que fizemos)
        const response = await api.get(`http://localhost:5000/api/presente/presentes_cha/?chaDeBebeId=${chaId}`);
        console.log(response);
        detalhes.value = response.data;
        console.log(detalhes);
    } finally {
        loading.value = false;
    }
};

const displayModal = ref(false);
const enviando = ref(false);

const novoPresente = ref({
    nome: '',
    descricao: '',
    preco: 0,
    quantidadeTotal: 1,
    linkSugerido: '',
    pathImage: '' // Opcional por enquanto
});

const salvarPresente = async () => {
    enviando.value = true;
    var response = null;
    try {
        // Importante: O Presente precisa estar vinculado ao ChaId da URL
        const payload = { ...novoPresente.value, ChaDeBebeEventoId: parseInt(route.params.id) };

        if (editando.value) {
            // Chamada de Edição: PUT /api/presentes/{id}
            response = await api.post(`http://localhost:5000/api/presente/atualizar/${presenteIdAtual.value}`, payload);
            toast.add({ severity: 'success', summary: 'Atualizado', detail: 'Presente editado com sucesso!' });
        } else {
            // Chamada de Criação: POST /api/presentes
            response = await api.post('http://localhost:5000/api/presente/adicionar', payload);
            toast.add({ severity: 'success', summary: 'Criado', detail: 'Presente adicionado!' });
        }
        
        toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Presente adicionado!', life: 3000 });
        displayModal.value = false;
        buscarDetalhes(); // Recarrega a tabela de presentes
    } catch (error) {
        const errorMessage = error.response?.data?.message || error.message || 'Erro desconhecido';
        toast.add({ severity: 'error', summary: 'Erro', detail: `Falha ao salvar presente: ${errorMessage}`, life: 3000 });
    } finally {
        enviando.value = false;
    }
};

const confirm = useConfirm();

const excluirPresente = (id) => {
    confirm.require({
        message: 'Tem certeza que deseja remover este presente da lista?',
        header: 'Confirmar Exclusão',
        icon: 'pi pi-info-circle',
        rejectLabel: 'Cancelar',
        acceptLabel: 'Excluir',
        acceptClass: 'p-button-danger',
        accept: async () => {
            try {
                // DELETE /api/presentes/{id}
                await api.post(`http://localhost:5000/api/presente/deletar`, {ChaDeBebeEventoId: parseInt(route.params.id), presenteId:id});
                
                toast.add({ 
                    severity: 'success', 
                    summary: 'Removido', 
                    detail: 'Presente excluído com sucesso.', 
                    life: 3000 
                });
                
                // Atualiza a lista na tela
                buscarDetalhes(); 
            } catch (error) {
                toast.add({ 
                    severity: 'error', 
                    summary: 'Erro', 
                    detail: `Não foi possível excluir o presente. Erro: ${error.response?.data?.message || error.message || 'Erro desconhecido'}`,
                    life: 3000 
                });
            }
        }
    });
};

const editando = ref(false);
const presenteIdAtual = ref(null);

const abrirEdicao = (presente) => {
    editando.value = true;
    presenteIdAtual.value = presente.id;
    
    // Preenche o formulário com os dados existentes da linha da tabela
    novoPresente.value = { 
        nome: presente.nome,
        descricao: presente.descricao,
        preco: presente.preco,
        quantidadeTotal: presente.quantidadeTotal,
        linkSugerido: presente.linkSugerido
    };
    
    displayModal.value = true;
};

// Modifique a função de abrir criação para resetar o estado de edição
const abrirModalCriacao = () => {
    editando.value = false;
    presenteIdAtual.value = null;
    novoPresente.value = { nome: null, descricao: null, preco: null, quantidadeTotal: null, linkSugerido: null };
    displayModal.value = true;
};

const urlConvite = computed(() => `${window.location.origin}/convite/${detalhes.value?.id}`);

const copiarLink = () => {
    navigator.clipboard.writeText(urlConvite.value);
    toast.add({ severity: 'info', summary: 'Copiado', detail: 'Link pronto para enviar!', life: 2000 });
};

const arquivoSelecionado = ref(null);
const previewImagem = ref(null);
const displayModalImagem = ref(false);

// Função disparada quando o usuário escolhe um arquivo
const aoSelecionarArquivo = (event) => {
    const file = event.files[0]; // O PrimeVue coloca os arquivos em .files
    if (file) {
        arquivoSelecionado.value = file;
        // Cria uma URL temporária para mostrar a imagem na tela antes de subir
        previewImagem.value = URL.createObjectURL(file);
    }
};

const abrirModalImagem = (presente) => {
    displayModalImagem.value = true;
    presenteIdAtual.value = presente.id;
    arquivoSelecionado.value = null;
    previewImagem.value = null;
};

// Resetar ao fechar o modal
const fecharModalImagem = () => {
    displayModalImagem.value = false;
    arquivoSelecionado.value = null;
    previewImagem.value = null;
};

const uploadImagem = async () => {
    enviando.value = true;
    
    // Criamos o FormData
    const formData = new FormData();
    
    // Adicionamos os campos de texto
    formData.append('chaDeBebeEventoId', parseInt(route.params.id));
    formData.append('presenteId', presenteIdAtual.value);

    // Se houver um novo arquivo, adicionamos ao corpo
    if (arquivoSelecionado.value) {
        formData.append('arquivo', arquivoSelecionado.value);
    }

    try {
        await api.post(`http://localhost:5000/api/presente/adicionar-imagem`, formData, {
            headers: { 'Content-Type': 'multipart/form-data' }
        });

        toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Presente salvo com imagem!' });
        fecharModalImagem();
        buscarDetalhes();
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Erro', detail: 'Erro ao fazer upload.' });
    } finally {
        enviando.value = false;
    }
};

onMounted(buscarDetalhes);
</script>

<template>
    <div class="surface-card p-4 shadow-1 border-round mb-4 flex align-items-center justify-content-between bg-blue-50">
        <div class="flex align-items-center gap-3">
            <i class="pi pi-link text-blue-600 text-xl"></i>
            <div>
                <span class="block font-bold text-blue-900">Link de Convite</span>
                <code class="text-blue-700">{{ urlConvite }}</code>
            </div>
        </div>
        <Button icon="pi pi-copy" label="Copiar" class="p-button-sm p-button-info" @click="copiarLink" />
    </div>
    <Dialog v-model:visible="displayModalImagem" :header="'Upload Imagem'" :modal="true" class="p-fluid w-full max-w-30rem">
        <div class="field">
            <label class="font-bold">Imagem do Presente</label>
            
            <div v-if="previewImagem || novoPresente.pathImage" class="mb-3 flex justify-content-center">
                <img :src="previewImagem || novoPresente.pathImage" 
                    class="border-round shadow-2" 
                    style="width: 150px h-150px; object-fit: cover" />
            </div>

            <FileUpload 
                mode="basic" 
                name="foto" 
                accept="image/*" 
                :maxFileSize="1000000" 
                @select="aoSelecionarArquivo" 
                chooseLabel="Escolher Foto"
                class="p-button-outlined w-full"
            />
            <small class="text-500">Tamanho máximo: 1MB (JPG, PNG).</small>
        </div>
        <template #footer>
            <Button label="Cancelar" icon="pi pi-times" text @click="fecharModalImagem" :disabled="enviando" />
            <Button label="Enviar" icon="pi pi-upload" @click="uploadImagem" :loading="enviando" :disabled="!arquivoSelecionado" />
        </template>
    </Dialog>
    <Dialog v-model:visible="displayModal" :header="editando ? 'Editar Presente' : 'Novo Presente'" :modal="true" class="p-fluid w-full max-w-30rem">
        <div class="field">
            <label for="nome" class="font-bold">Nome do Presente</label>
            <InputText id="nome" v-model="novoPresente.nome" placeholder="Ex: Pacote de Fraldas P" autofocus />
        </div>

        <div class="field">
            <label for="desc" class="font-bold">Descrição (Opcional)</label>
            <Textarea id="desc" v-model="novoPresente.descricao" rows="2" />
        </div>

        <div class="formgrid grid">
            <div class="field col">
                <label for="preco" class="font-bold">Preço Sugerido</label>
                <InputNumber id="preco" v-model="novoPresente.preco" mode="currency" currency="BRL" locale="pt-BR" />
            </div>
            <div class="field col">
                <label for="qtd" class="font-bold">Quantidade</label>
                <InputNumber id="qtd" v-model="novoPresente.quantidadeTotal" showButtons :min="1" />
            </div>
        </div>

        <div class="field">
            <label for="link" class="font-bold">Link de Referência (Opcional)</label>
            <InputText id="link" v-model="novoPresente.linkSugerido" placeholder="https://amazon.com.br/..." />
        </div>

        <template #footer>
            <Button label="Cancelar" icon="pi pi-times" text @click="displayModal = false" :disabled="enviando" />
            <Button label="Salvar" icon="pi pi-check" @click="salvarPresente" :loading="enviando" :disabled="!novoPresente.nome || novoPresente.preco == null || novoPresente.quantidadeTotal == null" />
        </template>
    </Dialog>
    <div v-if="loading" class="flex justify-content-center p-8">
        <ProgressSpinner />
    </div>

    <div v-else-if="detalhes" class="p-4">
        <div class="flex flex-column md:flex-row gap-4 mb-5">
            <div class="surface-card p-4 shadow-1 border-round flex-1 border-left-3 border-blue-500">
                <span class="text-500 font-medium block mb-3">Evento</span>
                <div class="text-900 font-bold text-2xl">{{ detalhes.nome }}</div>
                <div class="text-500 mt-2"><i class="pi pi-calendar mr-2"></i>{{ new Date(detalhes.dataEvento).toLocaleDateString() }}</div>
            </div>

            <div class="surface-card p-4 shadow-1 border-round flex-1 border-left-3 border-green-500">
                <span class="text-500 font-medium block mb-3">Resumo de Presentes</span>
                <div class="flex align-items-center justify-content-between">
                    <div>
                        <div v-if="!loading && detalhes" class="p-4">
                            <div class="text-2xl">{{ detalhes.presentes.length }}</div>
                                <div class="text-900 font-bold text-2xl">{{ detalhes.presentes.length }}</div>
                            </div>

                            <div v-else class="flex justify-content-center p-8">
                                <ProgressSpinner />
                            </div>
                        
                        <div class="text-500">Total Cadastrado</div>
                    </div>
                    <i class="pi pi-gift text-4xl text-green-500"></i>
                </div>
            </div>
        </div>

        <div class="surface-card p-4 shadow-1 border-round">
            <div class="flex justify-content-between align-items-center mb-4">
                <h2 class="m-0 text-xl">Lista de Presentes</h2>
                <Button label="Adicionar Presente" icon="pi pi-plus" size="small" severity="primary" @click="abrirModalCriacao" />
            </div>

            <DataTable :value="detalhes.presentes" responsiveLayout="stack" breakpoint="960px" stripedRows>
                <Column header="Imagem">
                    <template #body="slotProps">
                        <img v-if="slotProps.data.pathImage" 
                             :src="slotProps.data.pathImage" 
                             class="border-round" 
                             style="width: 50px; height: 50px; object-fit: cover" />
                        <span v-else class="text-500">-</span>
                    </template>
                </Column>
                <Column field="nome" header="Presente"></Column>
                <Column field="quantidadeTotal" header="Qtd. Total"></Column>
                <Column header="Restante">
                    <template #body="slotProps">
                        <Tag :value="slotProps.data.quantidadeRestante" 
                             :severity="slotProps.data.quantidadeRestante > 0 ? 'info' : 'danger'" />
                    </template>
                </Column>
                <Column field="linkSugerido" header="Link">
                    <template #body="slotProps">
                        <a v-if="slotProps.data.linkSugerido" :href="slotProps.data.linkSugerido" target="_blank" class="text-primary">
                            <i class="pi pi-external-link"></i>
                        </a>
                        <span v-else class="text-500">-</span>
                    </template>
                </Column>
                <Column header="Ações">
                    <template #body="slotProps">
                        <Button icon="pi pi-pencil" text rounded severity="secondary" 
                            @click="abrirEdicao(slotProps.data)"/>
                        <Button icon="pi pi-image" text rounded severity="secondary" 
                            @click="abrirModalImagem(slotProps.data)"/>
                        <Button icon="pi pi-trash" text rounded severity="danger"
                            @click="excluirPresente(slotProps.data.id)" />
                    </template>
                </Column>
            </DataTable>
        </div>
    </div>
</template>