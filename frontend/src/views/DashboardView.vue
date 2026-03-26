<script setup>
import { ref, onMounted } from 'vue';
import api from '@/services/api';
import { useToast } from "primevue/usetoast";

const toast = useToast();
const chas = ref([]);
const chas_inscrito = ref([]);
const loading = ref(true);
const displayModal = ref(false); // Controla o Modal de criação

// Dados do novo Chá
const novoCha = ref({
    nome: '',
    dataEvento: null
});

const carregarMeusChas = async () => {
    loading.value = true;
    try {
        const response = await api.get('http://localhost:5000/api/cha_de_bebe/meus_chas');
        chas.value = response.data;
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Erro', detail: 'Não foi possível carregar os eventos.', life: 3000 });
    } finally {
        loading.value = false;
    }
};

const carregarChasInscrito = async () => {
    loading.value = true;
    try {
        const response = await api.get('http://localhost:5000/api/cha_de_bebe/chas_inscrito');
        chas_inscrito.value = response.data;
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Erro', detail: 'Não foi possível carregar os eventos.', life: 3000 });
    } finally {
        loading.value = false;
    }
};

const criarEvento = async () => {
    try {
        await api.post('http://localhost:5000/api/cha_de_bebe/criar', novoCha.value); // Ajuste para sua rota de POST
        toast.add({ severity: 'success', summary: 'Sucesso', detail: 'Chá de Bebê criado!', life: 3000 });
        displayModal.value = false;
        novoCha.value = { nome: '', dataEvento: null }; // Reseta o form
        carregarMeusChas(); // Atualiza a lista
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Erro', detail: 'Erro ao criar evento.', life: 3000 });
    }
};

onMounted(carregarMeusChas);
onMounted(carregarChasInscrito);
</script>

<template>
    <div class="p-4">
        <div class="flex justify-content-between align-items-center mb-4">
            <h1 class="text-2xl font-bold m-0 text-900">Meus Eventos</h1>
            <Button label="Novo Chá" icon="pi pi-plus" @click="displayModal = true" />
        </div>


        <Dialog v-model:visible="displayModal" header="Novo Chá de Bebê" :modal="true" class="p-fluid w-full max-w-25rem">
            <div class="field mt-2">
                <label for="nome">Nome do Evento</label>
                <InputText id="nome" v-model="novoCha.nome" placeholder="Digite o nome..." />
            </div>
            <div class="field">
                <label for="data">Data do Evento</label>
                <Calendar id="data" v-model="novoCha.dataEvento" dateFormat="dd/mm/yy" showIcon />
            </div>
            <template #footer>
                <Button label="Cancelar" icon="pi pi-times" text @click="displayModal = false" />
                <Button label="Criar Chá" icon="pi pi-check" @click="criarEvento" :disabled="!novoCha.nome" />
            </template>
        </Dialog>
        
        <div class="flex gap-6">
            <!-- Meus Chás -->
            <div>
                <h2 class="text-lg font-bold mb-4 text-900">Meus Chás</h2>
                <div v-if="loading" class="flex justify-content-center p-8">
                    <ProgressSpinner />
                </div>

                <div v-else-if="chas.length === 0" class="text-center p-8 border-2 border-dashed surface-border border-round">
                    <i class="pi pi-calendar-minus text-4xl text-400 mb-3"></i>
                    <p class="text-xl text-500">Você ainda não tem chás cadastrados.</p>
                    <Button label="Começar agora" icon="pi pi-plus" class="p-button-text" @click="displayModal = true" />
                </div>

                <div v-else class="grid">
                    <div v-for="cha in chas" :key="cha.id" 
                    class="surface-card p-4 shadow-1 border-round border-left-3 border-primary hover:shadow-3 transition-duration-200 flex flex-column md:flex-row align-items-center justify-content-between gap-5">
                    
                    <div class="flex align-items-center gap-5 w-full md:w-auto">
                        <div>
                        <h3 class="text-xl font-bold m-0 text-900">{{ cha.nome }}</h3>
                        <div class="flex gap-3 mt-1 text-600">
                            <span><i class="pi pi-clock mr-1"></i> {{ new Date(cha.dataEvento).toLocaleDateString() }}</span>
                            <span><i class="pi pi-ticket mr-1"></i> Código: <b class="text-primary">{{ cha.id || '---' }}</b></span>
                        </div>
                        </div>
                    </div>

                    <div class="flex flex-column align-items-start md:align-items-center w-full md:w-auto">
                        <span class="text-sm text-500 mb-1 font-medium">PRESENTES</span>
                        <span class="text-2xl font-bold text-900">{{ cha.qtdPresentes }}</span>
                    </div>

                    <div class="flex gap-2 w-full md:w-auto justify-content-end">
                        <Button label="Gerenciar" icon="pi pi-cog" 
                            class="p-button-outlined p-button-secondary" 
                            @click="$router.push(`/gerenciar/${cha.id}`)" />
                        
                        <Button icon="pi pi-share-alt" 
                            v-tooltip.top="'Copiar Link'" 
                            severity="primary" />
                    </div>
                    </div>
                </div>
            </div>

            <!-- Chás Inscritos -->
            <div>
            <h2 class="text-lg font-bold mb-4 text-900">Chás que Estou Inscrito</h2>
                <div v-if="loading" class="flex justify-content-center p-8">
                    <ProgressSpinner />
                </div>

                <div v-else-if="chas_inscrito.length === 0" class="text-center p-8 border-2 border-dashed surface-border border-round">
                    <i class="pi pi-calendar-minus text-4xl text-400 mb-3"></i>
                    <p class="text-xl text-500">Você não está inscrito em nenhum chá.</p>
                </div>

                <div v-else class="grid">
                    <div v-for="cha in chas_inscrito" :key="cha.id" 
                    class="surface-card p-4 shadow-1 border-round border-left-3 border-primary hover:shadow-3 transition-duration-200 flex flex-column md:flex-row align-items-center justify-content-between gap-4">
                    
                    <div class="flex align-items-center gap-4 w-full md:w-auto">
                        <div>
                        <h3 class="text-xl font-bold m-0 text-900">{{ cha.nome }}</h3>
                        <div class="flex gap-3 mt-1 text-600">
                            <span><i class="pi pi-clock mr-1"></i> {{ new Date(cha.dataEvento).toLocaleDateString() }}</span>
                            <span><i class="pi pi-ticket mr-1"></i> Código: <b class="text-primary">{{ cha.id || '---' }}</b></span>
                        </div>
                        </div>
                    </div>

                    <div class="flex flex-column align-items-start md:align-items-center w-full md:w-auto">
                        <span class="text-sm text-500 mb-1 font-medium">PRESENTES</span>
                        <span class="text-2xl font-bold text-900">{{ cha.qtdPresentes }}</span>
                    </div>

                    <div class="flex gap-2 w-full md:w-auto justify-content-end">
                        <Button label="Ver Detalhes" icon="pi pi-eye" 
                            class="p-button-outlined p-button-secondary" 
                            @click="$router.push(`/cha_de_bebe/${cha.id}`)" />
                    </div>
                </div>
            </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
    .div_cha {
        display: flex;
        flex: 1;
    }
</style>