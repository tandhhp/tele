import { apiCallOptions } from "@/services/call";
import { DrawerForm, DrawerFormProps, ProFormSelect } from "@ant-design/pro-components"

type Props = DrawerFormProps & {
    data?: any;
}

const CallForm: React.FC<Props> = (props) => {
    return (
        <DrawerForm {...props} title="Cuộc gọi">
            <ProFormSelect name={`callStatusId`} label="Trạng thái" request={apiCallOptions} showSearch
                rules={[
                    {
                        required: true
                    }
                ]} />
        </DrawerForm>
    )
}

export default CallForm;