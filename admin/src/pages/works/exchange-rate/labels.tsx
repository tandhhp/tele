import { saveArguments } from "@/services/work-content"
import { ProForm, ProFormText } from "@ant-design/pro-components"
import { useParams } from "@umijs/max"
import { message } from "antd";

const ExchangeRateLabels: React.FC = () => {

    const { id } = useParams();

    const onFinish = async (values: any) => {
        const response = await saveArguments(id, values);
        if (response.succeeded) {
            message.success('Saved!');
        }
    }

    return (
        <>
            <ProForm onFinish={onFinish}>
                <ProFormText label="Sell" name="sell" />
            </ProForm>
        </>
    )
}

export default ExchangeRateLabels