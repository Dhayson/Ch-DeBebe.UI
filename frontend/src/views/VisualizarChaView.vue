<script setup>
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';
import api from '@/services/api';
import { useConfirm } from "primevue/useconfirm";
import { useToast } from "primevue/usetoast";

const route = useRoute();
const confirm = useConfirm();
const toast = useToast();

const cha_presentes = ref(null);
const loading = ref(true);
const win = window;

const carregarDados = async () => {
    try {
        const response = await api.get(`http://localhost:5000/api/presente/presentes_cha/?chaDeBebeId=${route.params.id}`);
        cha_presentes.value = response.data;
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Erro', detail: 'Evento não encontrado.', life: 3000 });
    } finally {
        loading.value = false;
    }
};

const displayReservaModal = ref(false);
const presenteSelecionado = ref(null);
const quantidadeReserva = ref(0.0);
const enviandoReserva = ref(false);

const prepararReserva = (presente) => {
    presenteSelecionado.value = presente;
    quantidadeReserva.value = 1.0; // Reseta para 1 como padrão
    displayReservaModal.value = true;
};

const confirmarReserva = async () => {
    if (quantidadeReserva.value > presenteSelecionado.value.quantidadeRestante) {
        toast.add({ severity: 'error', summary: 'Quantidade inválida', detail: 'Você não pode reservar mais do que o disponível.' });
        return;
    }

    enviandoReserva.value = true;
    try {
        // Enviamos o ID do presente e a quantidade escolhida
        await api.post(`http://localhost:5000/api/reserva/adicionar`, {
            PresenteId: presenteSelecionado.value.id,
            Quantidade: quantidadeReserva.value,
            UsuarioId: localStorage.getItem('usuarioId'),
            ChaDeBebeEventoId: route.params.id,
            DataReserva: new Date().toISOString(),
        });

        toast.add({ severity: 'success', summary: 'Sucesso!', detail: 'Reserva confirmada. Obrigado!', life: 3000 });
        displayReservaModal.value = false;
        carregarDados(); // Atualiza a lista para refletir a nova quantidade restante
    } catch (error) {
        toast.add({ severity: 'error', summary: 'Erro', detail: `Falha ao processar reserva. Erro: ${error}`, life: 3000 });
    } finally {
        enviandoReserva.value = false;
    }
};
const irParaLoja = (url) => {
    if (url) {
        window.open(url, '_blank', 'noopener,noreferrer');
    } else {
        toast.add({ severity: 'warn', summary: 'Link indisponível' });
    }
};

onMounted(carregarDados);
</script>

<template>
    <div v-if="loading" class="flex justify-content-center p-8"><ProgressSpinner /></div>

    <div v-else-if="cha_presentes" class="p-4 max-w-7xl mx-auto">
        <div class="surface-section text-center p-6 border-round-xl shadow-1 mb-6 bg-blue-50">
            <i class="pi pi-heart-fill text-pink-500 text-4xl mb-3"></i>
            <h1 class="text-4xl font-bold text-900 m-0">{{ cha_presentes.nome }}</h1>
            <p class="text-xl text-600 mt-2">
                <i class="pi pi-calendar mr-2"></i> {{ new Date(cha_presentes.dataEvento).toLocaleDateString() }}
            </p>
        </div>

        <div class="grid">
            <div v-for="p in cha_presentes.presentes" :key="p.id" class="lg:col-10 p-2">
                <Card class="h-full flex flex-column justify-content-between overflow-hidden">
                    <template #header>
                        <div class="relative">
                            <img :src="p.pathImage || 'https://placehold.co/400x300?text=Presente'" 
                                 class="w-full object-contain" />
                            <Tag v-if="p.quantidadeRestante === 0" value="Esgotado" severity="danger" class="absolute top-0 right-0 m-2" />
                        </div>
                    </template>
                    <template #title> {{ p.nome }} </template>
                    <template #subtitle> 
                        <span class="text-primary font-bold text-lg">
                            {{ p.preco.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' }) }}
                        </span>
                    </template>
                    <template #content>
                        <p class="m-0 text-600 text-sm line-height-3"> {{ p.descricao }} </p>
                        <div class="mt-3 flex align-items-center justify-content-between">
                            <span class="text-sm font-medium">Disponível: {{ p.quantidadeRestante }}</span>
                        </div>
                    </template>
                    <template #footer>
                        <Button 
                            label="Eu quero dar este!" 
                            icon="pi pi-gift" 
                            class="w-full p-button-raised" 
                            :disabled="p.quantidadeRestante === 0"
                            @click="prepararReserva(p)" 
                        />
                        <Button 
                            v-if="p.linkSugerido" 
                            label="Ver na Loja" 
                            icon="pi pi-external-link" 
                            class="p-button-text w-full mt-2" 
                            @click="irParaLoja(p.linkSugerido)" 
                        />
                    </template>
                </Card>
            </div>
        </div>
    </div>

    <Dialog v-model:visible="displayReservaModal" header="Reservar Presente" :modal="true" class="p-fluid w-full max-w-20rem">
        <div class="text-center mb-4">
            <p class="m-0 font-semibold text-xl">{{ presenteSelecionado?.nome }}</p>
            <small class="text-500">Disponível: {{ presenteSelecionado?.quantidadeRestante }}</small>
        </div>

        <div class="field text-center">
            <label for="qtdReserva" class="block mb-2 font-bold ">Quantas unidades deseja dar?</label>
            <label for="qtdReserva" class="block mb-1">(Pode digitar um valor fracional para dividir o custo)</label>
            <InputNumber 
                id="qtdReserva" 
                v-model="quantidadeReserva" 
                showButtons 
                buttonLayout="horizontal" 
                :min="0.0" 
                :max="presenteSelecionado?.quantidadeRestante"
                incrementButtonIcon="pi pi-plus" 
                decrementButtonIcon="pi pi-minus"
                inputClass="text-center"
                :minFractionDigits="0"
                :maxFractionDigits="2"
            />
        </div>

        <template #footer>
            <Button label="Cancelar" icon="pi pi-times" text @click="displayReservaModal = false" />
            <Button label="Confirmar" icon="pi pi-check" @click="confirmarReserva" :loading="enviandoReserva" />
        </template>
    </Dialog>
</template>